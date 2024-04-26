// File create date:2024/4/18
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public abstract class BasePawnController : HierarchyStateMachine  {

	public enum PawnState {
		Idle,
		Move,
		Attack,
		Think,
		Dead
	}

	protected SpriteRenderer avatarSprite;
	protected AnimatorStateMachine animatorMachine;
	protected BasePawnRobot robot;

	protected GameDataService gameData;
	
	public int Id { get; private set; }
	public Square Coord { get; protected set; }
	public Square Area { get; protected set; }

	protected override void OnMachineAwake() {
		base.OnMachineAwake();
		avatarSprite = FindComponent<SpriteRenderer>("Avatar");
		animatorMachine = avatarSprite.GetComponent<AnimatorStateMachine>();
	}

	protected override void OnMachineStart() {
		base.OnMachineStart();
		gameData = GameSystem.Instance.GameData;
		robot = CreateRobot();
		robot?.Initialize();
		SetupMachine();
	}

	protected abstract BasePawnRobot CreateRobot();

	protected virtual void SetupMachine() {
		SetupState(PawnState.Idle);
		SetupState(PawnState.Move);
		SetupState(PawnState.Attack);
		SetupState(PawnState.Think);
		SetupState(PawnState.Dead);
		SetupEntryState(PawnState.Idle);
	}

	protected override void OnLazyStart() {
		base.OnLazyStart();
		StartMachine();
	}

	protected override void OnMachineUpdate() {
		base.OnMachineUpdate();
		robot?.OnRobotUpdate();
	}

	public void SetupIdentifier(int id) {
		Id = id;
	}

	public void NotifyGenerate() {
		YieldAction(OnPawnGenerated);
	}

	protected virtual void OnPawnGenerated() {
		gameData.stageEntityData.RegisterPawn(this);
	}

	protected override void Release() {
		base.Release();
		gameData.stageEntityData.UnregisterPawn(this);
	}
}