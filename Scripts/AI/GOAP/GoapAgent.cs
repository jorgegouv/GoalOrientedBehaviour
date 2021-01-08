using System;
using System.Collections.Generic;
using System.Linq;
using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP.HSM.Actions;
using Assets.GoalOrientedBehaviour.Scripts.AI.GOAP.HSM.Conditions;
using Assets.Scripts.HSM.Abstracts;
using Assets.Scripts.HSM.Concretes;
using UnityEngine;

namespace Assets.GoalOrientedBehaviour.Scripts.AI.GOAP
{
    /// <summary>
    /// The GOAP Agent. Stores information about the available, current plan, state machine. Every Update, it checks the transition of its state machine. This machine has 3 states: it can be planning, moving or action. Every state can transition to every other state.
    /// </summary>
    public sealed class GoapAgent : MonoBehaviour
    {
        /// <summary>
        /// The name of the current state in the state machine. Used for visualization only.
        /// </summary>
        public string CurrentState;
        /// <summary>
        /// The state machine this agent uses.
        /// </summary>
        private HierarchicalStateMachine _hsm;
        /// <summary>
        /// All available actions for this agent
        /// </summary>
        private HashSet<IGoapAction> _availableActions;
        /// <summary>
        /// The action sequence of the current plant. If this queue is empty, then the agent has no plan.
        /// </summary>
        private Queue<IGoapAction> _currentActions;
        /// <summary>
        /// This is the implementing class that provides our world data and listens to feedback on planning
        /// </summary>
        private IGoap _dataProvider; 
        /// <summary>
        /// Reference to the planner.
        /// </summary>
        private GoapPlanner _planner;
        
        /// <summary>
        /// Property that indicates if this agent needs to move. It will be true if we have a plan and we are not in range of the next action's target.
        /// </summary>
        public bool NeedToMove
        {
            get
            {
                if (_currentActions.Count == 0)
                    return false;

                return (_currentActions.Peek().RequiresInRange() == false || _currentActions.Peek().InRange) == false;
            }
        }
        /// <summary>
        /// Property that indicates if we need a new plan. We need a new plan if there is no actions in the <see cref="_currentActions"/> queue.
        /// </summary>
        public bool NeedNewPlan { get{ return _currentActions.Count == 0; } }

        /// <summary>
        /// perform all the agent's initializations
        /// </summary>
        private void Awake()
        {
            _availableActions = new HashSet<IGoapAction>();
            _currentActions = new Queue<IGoapAction>();
            _planner = new GoapPlanner();

            FindDataProvider();
            LoadActions();

            InitializeHsm();


        }

        /// <summary>
        /// Updates the state machine. This is done in the FixedUpdate to allow the movement action to be synchronized with the physics engine.
        /// </summary>
        private void FixedUpdate()
        {
            UpdateHsm();
        }

        /// <summary>
        /// Initializes the <see cref="HierarchicalStateMachine"/> for this <see cref="GoapAgent"/>. It has 3 states: Planning, Moving or Acting. Every state can transition to every other state. Check the conditions to see what triggers transitions: <see cref="NeedToMoveCondition"/>, <see cref="NeedNewPlanCondition"/> and <see cref="ReadyToPerformActionCondition"/>.
        /// </summary>
        private void InitializeHsm()
        {
            //print("initializing hsm");
            
            // create the states
            var statePlanning = new State("Planning")
            {
                ActiveActions = new HashSet<IAction> {new PlanAction(_planner, _dataProvider, this)},
                ExitActions = new HashSet<IAction>() // no exit actions
            };
            var stateMoving = new State("Moving")
            {
                ActiveActions = new HashSet<IAction> {new MoveToAction(this, _dataProvider)},
                ExitActions = new HashSet<IAction>() // no exit actions
            };
            var stateActing = new State("Acting")
            {
                ActiveActions = new HashSet<IAction> {new PerformActionAction(this, _dataProvider)},
                ExitActions = new HashSet<IAction>() // no exit actions
            };

            // create the state machine and add the states to it
            _hsm = new HierarchicalStateMachine(statePlanning, "goap hsm")
            {
                States = new HashSet<IState>{statePlanning, stateMoving, stateActing},
                ActiveActions = new HashSet<IAction>(), // no active actions
                EntryActions = new HashSet<IAction>(), // no entry actions
                ExitActions = new HashSet<IAction>() // no exit actions
            };
            

            // inform the states which is the state machine they belong to
            statePlanning.Parent = _hsm;
            stateMoving.Parent = _hsm;
            stateActing.Parent = _hsm;

            // initialize the transtion conditions
            var needToMoveCondition = new NeedToMoveCondition();
            var needNewPlanCondition = new NeedNewPlanCondition();
            var readyToActCondition = new ReadyToPerformActionCondition();

            // define the conditions
            var transPlanToMoveTo = new Transition(
                0, // should always be zero for a single layer state machine
                new HashSet<IAction>(),
                stateMoving,
                "transPlanToMoveTo",
                new List<ICondition> {needToMoveCondition},
                this);

            var transPlanToAct = new Transition(
                0, 
                new HashSet<IAction>(),
                stateActing,
                "transPlanToAct",
                new List<ICondition> {readyToActCondition},
                this);

            var transMoveToAct = new Transition(
                0, 
                new HashSet<IAction>(),
                stateActing,
                "transMoveToAct",
                new List<ICondition> {readyToActCondition},
                this);

            var transActToMove = new Transition(
                0, 
                new HashSet<IAction>(),
                stateMoving,
                "transActToMove",
                new List<ICondition> {needToMoveCondition},
                this);

            var transActToPlan = new Transition(
                0, 
                new HashSet<IAction>(),
                statePlanning,
                "transActToPlan",
                new List<ICondition> {needNewPlanCondition},
                this);

            var transMoveToPlan = new Transition(
                0, 
                new HashSet<IAction>(),
                statePlanning,
                "transMoveToPlan",
                new List<ICondition> {needNewPlanCondition},
                this);

            // add the conditions to each state
            statePlanning.Transitions = new HashSet<ITransition>{transPlanToMoveTo, transPlanToAct};
            stateMoving.Transitions = new HashSet<ITransition>{transMoveToPlan, transMoveToAct};
            stateActing.Transitions = new HashSet<ITransition>{transActToMove, transActToPlan};
        }

        /// <summary>
        /// Update teh state machine
        /// </summary>
        private void UpdateHsm()
        {
            foreach (var action in _hsm.Update().Actions)
            {
                action.Execute();
            }

            // Update the CurrentState name so we can vizualize it on the editor
            CurrentState = _hsm.CurState.Name;
        }

        /// <summary>
        /// Add an action to the agent's <see cref="_availableActions"/> list. Not used for this demo, but you can use this to change the agent's behaviour on the fly. Returns false if this agent already has this action.
        /// </summary>
        /// <param name="a"></param>
        public bool AddAction(IGoapAction a)
        {
            return _availableActions.Add(a);
        }

        /// <summary>
        /// Returns the first action of a certain type existing in this agent <see cref="_availableActions"/>. Returns null if there is none.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IGoapAction GetAction(Type action)
        {
            return _availableActions.FirstOrDefault(g => g.GetType() == action);
        }

        /// <summary>
        /// Removes the received action from the <see cref="_availableActions"/> list. Returns false if the agent did not had this action on its list.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool RemoveAction(IGoapAction action)
        {
            return _availableActions.Remove(action);
        }

        /// <summary>
        /// Get the <see cref="IGoap"/> component in this game object
        /// </summary>
        private void FindDataProvider()
        {
            _dataProvider = GetComponent<IGoap>();
        }

        /// <summary>
        /// Load all available actions for this agent. Since the <see cref="_availableActions"/> is a <see cref="HashSet{T}"/>, only the new actions will be added.
        /// </summary>
        private void LoadActions()
        {
            var actions = GetComponents<GoapAction>();
            if (actions == null) return;

            _availableActions.UnionWith(actions);
            
            Debug.Log("Found actions: " + PrettyPrint(actions));
        }

        /// <summary>
        /// prints the state in the console
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string PrettyPrint(IEnumerable<KeyValuePair<string, object>> state)
        {
            var s = "";
            foreach (var kvp in state)
            {
                s += kvp.Key + ":" + kvp.Value;
                s += ", ";
            }
            return s;
        }

        /// <summary>
        /// Prints the action in the console
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public static string PrettyPrint(IEnumerable<IGoapAction> actions)
        {
            var s = "";
            foreach (var a in actions)
            {
                s += a.GetType().Name;
                s += "-> ";
            }
            s += "GOAL";
            return s;
        }

        /// <summary>
        /// Prints the action in the console
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public static string PrettyPrint(IGoapAction[] actions)
        {
            var s = "";
            foreach (var a in actions)
            {
                s += a.GetType().Name;
                s += ", ";
            }
            return s;
        }

        /// <summary>
        /// Prints the action in the console
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string PrettyPrint(IGoapAction action)
        {
            var s = "" + action.GetType().Name;
            return s;
        }

        /// <summary>
        /// Gets all the available actions for this agent
        /// </summary>
        /// <returns></returns>
        public HashSet<IGoapAction> GetAvailableActions()
        {
            return _availableActions;
        }

        /// <summary>
        /// Set the current actions of this agent to be equal to the received queue
        /// </summary>
        /// <param name="actions"></param>
        public void SetCurrentActions(Queue<IGoapAction> actions)
        {
            _currentActions = actions;
        }

        /// <summary>
        /// Get the current actions for this agent
        /// </summary>
        /// <returns></returns>
        public Queue<IGoapAction> GetCurrentActions()
        {
            return _currentActions;
        }

        /// <summary>
        /// Peek at the next action in this agent's plan
        /// </summary>
        /// <returns></returns>
        public IGoapAction PeekNextAction()
        {
            return _currentActions.Peek();
        }

        /// <summary>
        /// Aborts the current plan.
        /// </summary>
        public void AbortPlan()
        {
            _currentActions.Clear();
        }

        /// <summary>
        /// Returns the top action from the actions queue. Returns null if there are no actions.
        /// </summary>
        public IGoapAction GetCurrentAction()
        {
            return _currentActions.Count > 0 ? _currentActions.Peek() : null;
        }
    }
}
