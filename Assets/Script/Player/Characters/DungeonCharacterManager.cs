using System.Collections.Generic;
using UnityEngine;

public class DungeonCharacterManager : CharacterManager
{
    #region Setup
    public DungeonComponent dungeonRoom;
    public void PrepareTeamForDungeon(DungeonComponent newdungeon, Mob[] party)
    {
        heroes.Clear();
        heroes.AddRange(party);

        dungeonRoom = newdungeon;
        TransitionDungeon();
    }
    public void TransitionDungeon()
    {
        foreach (Mob hero in heroes)
        {
            GridNav.Node arrivalPoint = dungeonRoom.grid.RandomNodeInCircle(dungeonRoom.ArrivalPoint.transform.position, heroes.Count);
            hero.transition.MovePlayerToNewRoom(dungeonRoom, arrivalPoint);
            CameraController.main.JumptoMob(hero);
        }
        ClearSelection();
        InitLocalVars();
    }
    #endregion
    private void Update()
    {
        HandleInput();
        HandleGestures();
        CheckDefeat();
    }
    public float CameraSpeed = 2;
    public float CameraDragDistance = 2;
    public float BacklineDistance = 2;
    public void HandleAdvance(float advanceDistance)
    {
        Vector3 nCenter = CameraController.main.transform.position + Vector3.right * CameraSpeed * advanceDistance;

        CameraController.main.MovePosition(nCenter);
        //CameraController.main.DungeonCameraBehavior();


        foreach (Mob mob in heroes)
        {
            float backLine = mob.archetype == MobDefines.Archetype.melee ? 0 : -BacklineDistance;
            mob.attitude.WatchPoint(AttitudeComponent.Attitude.Guard, new Vector2(nCenter.x + backLine, mob.transform.position.y));

            //mob.orders.GiveOrder(new OrdersComponent.MoveOrder(mob.transform.position + Vector3.right * AdvanceDistance), 100);
        }
    }
    #region Read Gestures
    float gestureStart = 0;
    float gestureUpdate = 0;
    float lastgestureTime = 0;

    float doubleTapInterval = .33f;
    float shortTouch = .15f;
    float medTouch = 1; //TODO define

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.Menu))
        {
            GeneralUIManager.main.ShowPauseWindow();
        }
    }
    Vector2 fingerPosition;
    void HandleGestures()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {
                fingerPosition = CameraController.main.ScreenToWorldPoint(touch.position);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (InterfaceParent.CheckMouseOverUI(touch.position))
                        {
                            focus = GesturePhase.canceled;
                        }
                        else
                        {
                            OnGestureBegin();
                            originalPosition = touch.position;  //FIX bandaid fix
                        }
                        break;
                    case TouchPhase.Moved:
                        if (focus == GesturePhase.canceled) return;
                        else if (focus == GesturePhase.cameradrag)
                        {
                            MoveCamera(touch.position);
                            return;
                        }
                        else if (focus == GesturePhase.nothing)
                        {
                            float deltaTime = Time.time - gestureStart;
                            if (deltaTime >= shortTouch)
                            {
                                HandleLongTap( deltaTime < medTouch, true);
                            }
                            if (deltaTime < shortTouch && MobsGesturedOver.Count == 0  )
                            {
                                if ((touch.position - originalPosition).sqrMagnitude > CameraDragDistance * CameraDragDistance)
                                focus = GesturePhase.cameradrag;
                            }
                            else if (focus!=GesturePhase.gesturing)
                            {
                                InitOrderAssistant();
                                OrderAssistantUpdate();
                                CameraController.main.MoveCameraWithBorders(touch.position, CameraSpeed * Time .fixedDeltaTime * .33f);
                                focus = GesturePhase.gesturing;
                            }
                        }
                        else
                        if (gestureUpdate < Time.time)
                        {
                            gestureUpdate = Time.time + shortTouch;
                            UpdateMobsUnderFinger();
                            if (focus != GesturePhase.gesturing)
                            {
                                HandleLongTap(false, false);
                            }
                        }
                        OrderAssistantUpdate();
                        break;
                    case TouchPhase.Stationary:
                        if (focus == GesturePhase.canceled) return;
                        if (focus == GesturePhase.cameradrag)
                        {
                            MoveCamera(touch.position);
                            return;
                        }
                        if (focus != GesturePhase.gesturing && gestureUpdate < Time.time)
                        {
                            gestureUpdate = Time.time + shortTouch;
                            HandleLongTap( false, false);
                        }
                        if (focus == GesturePhase.gesturing)
                            {
                            CameraController.main.MoveCameraWithBorders(touch.position, CameraSpeed * Time.fixedDeltaTime * .33f);
                        }
                        break;
                    case TouchPhase.Ended:
                        if (focus == GesturePhase.canceled)
                        {
                            return;
                        } else if (focus == GesturePhase.gesturing)
                        {
                            GestureConclude();
                        }
                        else
                        {
                            float deltaTime = Time.time - gestureStart;
                            if (deltaTime < shortTouch)
                            {
                                OnShortTap(fingerPosition);
                            }
                            else
                            {
                                HandleLongTap( deltaTime < medTouch, true);
                            }
                        }
                        gestureStart = -1;
                        lastgestureTime = Time.time;

                        if (OrderAssistant.main != null)
                        {
                            OrderAssistant.main.enabled = false;
                        }
                        break;
                }
            }
        }
    }
    Mob targetMob;
    #endregion
    #region Taps
    void OnShortTap(Vector2 tapPosition)
    {
        bool doubleTap = Time.time - lastgestureTime < doubleTapInterval;

        if (doubleTap)
        {
            //Doubletap - select all heroes next to tap
            CircleSelect(tapPosition, SelectionDistance);
            if (SelectionCirclePrefab != null && SpecialEffectPool.main != null)
            {
                SpecialEffectPool.main.EffectFromPrefab(SelectionCirclePrefab, tapPosition, scale: SelectionDistance);
            }
        }
        else
        {
            targetMob = CameraController.main.MobFromWorldPoint(tapPosition);
            if (targetMob != null)
            {
                if (targetMob.IsPlayerControlled()) //tap once on ally - select ally
                {
                    ClearSelection();
                    SelectCharacter(targetMob);
                }
                
            }
            else //tap once on ground - clear selection
            {
                ClearSelection();
            }
        }
    }
    void HandleLongTap(bool absolute, bool final)
    {
        targetMob = CameraController.main.MobFromWorldPoint(fingerPosition);

        if (focus == GesturePhase.nothing)
        {
            if (targetMob != null)
            {
                if (targetMob.IsPlayerControlled())
                {
                    focus = GesturePhase.heal;
                }
                else
                {
                    focus = GesturePhase.snipe;
                }
                lastFocusTarget = targetMob;
            }
            else
            {
                focus = GesturePhase.retreat;
            }
        }
        switch (focus)
        {
            case GesturePhase.heal:// hold on ally - all supports heal ally(hold short for them to focus permanently)
                Focus(lastFocusTarget, MobDefines.Archetype.support, AbilityDefines.AbilityType.heal, absolute, final);
                break;
            case GesturePhase.snipe: // hold on enemy - all archers attack enemy(hold short for them to focus permanently)
                Focus(lastFocusTarget, MobDefines.Archetype.range, AbilityDefines.AbilityType.snipe, absolute, final);
                break;
            case GesturePhase.retreat:// hold on ground - all heroes run there(hold longer for them to return)	
                IssueLongGestureOrder(new OrdersComponent.MoveOrder(fingerPosition), absolute, final);
                break;
        }
        if (OrderAssistant.main != null)
        {
            OrderAssistant.main.enabled = false;
        }
    }
    #endregion
    #region Circle Selection
    [Header ("Circle Selection")]
    public float SelectionDistance = 3.5f;
    public GameObject SelectionCirclePrefab;
    void CircleSelect(Vector2 position, float range)
    {
        ClearSelection();
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(position, range, Vector2.zero))
        {
            if (hit.transform.TryGetComponent(out Mob target))
            {
                SelectCharacter(target);
            }
        }
    }
    #endregion
    #region Gestures
    MobList<Mob> MobsGesturedOver = new MobList<Mob>();
    void OnGestureBegin()
    {
        gestureStart = Time.time;
        gestureUpdate = Time.time + shortTouch;
        focus = GesturePhase.nothing;
        originalPosition = fingerPosition;
        MobsGesturedOver.Clear();
        UpdateMobsUnderFinger(true);

    }
    void InitOrderAssistant()
    {

        if (OrderAssistant.main != null)
        {
            OrderAssistant.main.enabled = true;
            if (targetMob != null && !targetMob.IsSelected())
            {
                OrderAssistant.main.RegisterSingleMob(targetMob);
            }
            else
            {
                OrderAssistant.main.RegisterHeroes(targetMob);
            }
        }
    }
    void UpdateMobsUnderFinger(bool playerOnly = false)
    {
        targetMob = CameraController.main.MobFromWorldPoint(fingerPosition, playerOnly);
        if (targetMob != null)
        {
            if (!MobsGesturedOver.Contains(targetMob))
            {
                MobsGesturedOver.Add(targetMob);
            }
        }
    }
    void OrderAssistantUpdate()
    {
        if (targetMob != null)
        {
                OrderAssistant.main.HighlightMob(targetMob);
            }
            else
            {
                OrderAssistant.main.HighlightPoint(fingerPosition);
        }

        }
    void GestureConclude()
    {
        if (MobsGesturedOver.Count > 0)
        {
            Mob firstMob = MobsGesturedOver[0];
            if (firstMob.IsPlayerControlled())
            {
                if (MobsGesturedOver.Count == 1 || targetMob == null)
                {
                    if (firstMob.IsSelected())
                    {
                        IssueGroupMoveOrder(firstMob.transform.position,fingerPosition, false,AttitudeComponent.Attitude.StandGround);
                    }
                    else
                    {
                        var order = new OrdersComponent.MoveOrder(fingerPosition);
                        firstMob.orders.ReplaceOrder(order);
                        firstMob.attitude.SetAttitude(AttitudeComponent.Attitude.Defensive);
                        OrderAssistant.main.DrawIndicatorForOrder(order);
                    }
                }
                else
                {
                    Mob lastMob = MobsGesturedOver[MobsGesturedOver.Count - 1];

                    if (MobsGesturedOver.Count == 2 && (lastMob.IsSelected() || !lastMob.IsPlayerControlled()))
                    {

                        if (firstMob.IsSelected())
                        {
                            IssueGroupAttackOrder(lastMob, false, AttitudeComponent.Attitude.Aggressive);
                        }
                        else if (firstMob.combatant.GetActiveAbility().IsValidTarget(lastMob))
                        {
                            OrdersComponent.AttackOrder order = new OrdersComponent.AttackOrder(lastMob);
                            firstMob.orders.ReplaceOrder(order);
                            OrderAssistant.main.DrawIndicatorForOrder(order);

                            firstMob.attitude.SetAttitude(AttitudeComponent.Attitude.Aggressive);
                        }
                    }
                    /*else DRAG SELECTION
                    {
                        ClearSelection();
                        foreach (Mob mob in MobsGesturedOver)
                        {
                            if (mob.IsPlayerControlled())
                                SelectCharacter(mob);

                        }
                    }*/
                }
            }
            /*else      DRAG ATTACK MULTIPLE UNITS
            {
                List<Mob> targets = new List<Mob>(); //TODO account for healers

                foreach (Mob mob in MobsGesturedOver)
                {
                    if (!mob.IsPlayerControlled())
                    {
                        targets.Add(mob);
                    }
                }
                OrdersComponent.AttackGroupOrder agOrder = new OrdersComponent.AttackGroupOrder(targets.ToArray());
                IssueGroupOrder(agOrder, false);
                ClearSelection();
                OrderAssistant.main.DrawIndicatorForOrder(agOrder);
            }*/
        }
    }

    #endregion
    #region Camera Move
    Vector2 originalPosition = Vector2.zero;
    Vector2 advanceVector;
    void MoveCamera(Vector2 touchPosition)
    {
        advanceVector = (touchPosition - originalPosition).normalized ;

        CameraController.main.MoveDirection(-advanceVector * CameraSpeed * Time.deltaTime );
        CameraController.main.DungeonCameraBehavior();
    }
    #endregion
    #region Selection
    public int GetNumSelectedHeroes()
    {
        int val = 0;
        foreach (Mob mob in heroes)
        {
            if (mob.IsSelected())
                val++;
        }

                return val;
    }
    public void SelectCharacter(Mob character)
    {
        if (!character.IsPlayerControlled()) return;
        character.Select();
        if (character is Hero selectedHero)
        {
            DungeonInterfaceController.main.SelectHero(selectedHero);
        }
    }
    public void SelectSingle(Mob character)
    {
        if (!character.IsPlayerControlled()) return;
        ClearSelection();
        character.Select();
        if (character is Hero selectedHero)
        {
            DungeonInterfaceController.main.SelectHero(selectedHero);
        }
    }
    public void ClearSelection()
    {
        foreach (Mob character in heroes)
        {
            character.Deselect();
        }
        DungeonInterfaceController.main.ClearSelection();
    }

    #endregion
    #region Orders
    void IssueGroupAttackOrder(Mob target, bool global, AttitudeComponent.Attitude attitudeOverride = AttitudeComponent.Attitude.Aggressive )
    {
        OrdersComponent.AttackOrder issuedOrder = null;
        foreach (Mob mob in heroes)
        {
            if (global || mob.IsSelected())
            {
                if (mob.combatant.GetActiveAbility().IsValidTarget(target))
                {
                    issuedOrder = new OrdersComponent.AttackOrder(target);
                    mob.orders.ReplaceOrder(issuedOrder);
                    mob.attitude.SetAttitude(attitudeOverride);
                }
            }
        }
        if (issuedOrder!= null)
        {
            OrderAssistant.main.DrawIndicatorForOrder(issuedOrder);
        }
    }
    void IssueGroupMoveOrder(Vector2 startingPos, Vector2 endPos, bool global, AttitudeComponent.Attitude attitudeOverride = AttitudeComponent.Attitude.Aggressive)
    {
        float CohesionDistanceThreshold = 2;//todo define
        float FormationDistance = 3;//todo define

        int selectedHeroes = GetNumSelectedHeroes();
        foreach (Mob mob in heroes)
        {
            if (global || mob.IsSelected())
            {
                Vector2 delta = ((Vector2)mob.transform.position - startingPos);

                if (delta.sqrMagnitude > CohesionDistanceThreshold* CohesionDistanceThreshold)
                {
                    delta = delta.normalized * FormationDistance * ((CohesionDistanceThreshold * CohesionDistanceThreshold) / delta.sqrMagnitude);
                }

                Vector2 destination = endPos + delta;

                OrdersComponent.MoveOrder moveOrder = new OrdersComponent.MoveOrder(destination,(selectedHeroes>1 && mob.archetype == MobDefines.Archetype.melee) ? 0 : BacklineDistance);

                mob.orders.ReplaceOrder(moveOrder);
                mob.attitude.SetAttitude(attitudeOverride);

                OrderAssistant.main.DrawIndicatorForOrder(moveOrder);
            }
        }
    }
    void IssueGroupOrder(OrdersComponent.MoveOrder order, bool global, AttitudeComponent.Attitude attitudeOverride = AttitudeComponent.Attitude.Aggressive)
    {
        foreach (Mob mob in heroes)
        {
            if (mob.CanRespond() && (global || mob.IsSelected()))
            {
                mob.orders.ReplaceOrder(order);
                mob.attitude.SetAttitude(attitudeOverride);
            }
        }
    }
    void GiveTempOrder(OrdersComponent.MoveOrder order)
    {
        foreach (Mob mob in heroes)
        {
            if (mob.CanRespond())
            {
                mob.orders.GiveTempOrder(order);
                mob.attitude.SetAttitude(AttitudeComponent.Attitude.Aggressive);
            }
        }
    }
    #endregion
    #region Focus Skills
    public enum GesturePhase
    {
        nothing,
        heal,
        snipe,
        retreat,
        gesturing,
        cameradrag,
        canceled,
    }
    GesturePhase focus;
    Mob lastFocusTarget = null;
    void IssueLongGestureOrder(OrdersComponent.MoveOrder order, bool absolute, bool final)
    {
        if (final)
        {
            if (absolute)
            {
                IssueGroupOrder(order, true, AttitudeComponent.Attitude.Aggressive);
            }
            foreach (Mob hero in heroes)
            {
                hero.orders.ClearTempOrder();
            }

        }
        else
        {
            GiveTempOrder(order);
        }
    }
    void Focus(Mob target, MobDefines.Archetype requiredAtype, AbilityDefines.AbilityType requiredAbility, bool absolute, bool final)
    {
        if (target != null)
        {
            foreach (Mob mob in heroes)
            {
                if (mob.archetype == requiredAtype)
                {
                    if (final)
                    {
                        if (absolute)
                        {
                            mob.orders.ReplaceOrder(new OrdersComponent.AttackOrder(target));
                        }
                        foreach (Mob hero in heroes)
                        {
                            hero.orders.ClearTempOrder();
                        }
                    }
                    else
                    {
                        if (!mob.orders.HasTempOrder())
                        {
                            PropertyAbility sepcificAbility = mob.abilities.FindAbilityByType(requiredAbility);
                            if (sepcificAbility != null && sepcificAbility.IsValidTarget(target))
                            {
                                mob.orders.GiveTempOrder(new OrdersComponent.CastOrder(target, sepcificAbility));
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region Dungeon Local Vars
    public Variables.VariableScope scope = new Variables.VariableScope();
    void InitLocalVars()
        {
        scope.ClearVars();
        }
    #endregion
    #region Defeat Check
    float lastDefeatCheck = 0;
    void CheckDefeat()
    {
        if (lastDefeatCheck < Time.time)
        {
            lastDefeatCheck = Time.time + 1;

            bool playerDefeated = true;
            foreach (Mob hero in heroes)
            {
                if (hero.damageable.isAlive())
                {
                    playerDefeated = false;
                }
            }
            if (playerDefeated)
            {
                DungeonFail();
            }
        }
    }

    #endregion
    #region Exit Dungeon
    public void DungeonFail()
    {
        GeneralUIManager.main.ShowDefeatWindow();
    }
    public void DungeonSucceed()
    {
        GeneralUIManager.main.ShowVictoryWindow();

    }
    #endregion
}
