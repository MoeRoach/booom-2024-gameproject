// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public abstract class BaseObjectControl : HierarchyStateMachine  {

	protected enum ObjectState {
		Idle
	}
	
	protected SpriteRenderer avatarSprite;
	protected AnimatorStateMachine animatorMachine;
	
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
	}
	
	public void SetupIdentifier(int id) {
		Id = id;
	}

	public void NotifyGenerate() {
		YieldAction(OnObjectGenerated);
	}

	protected virtual void OnObjectGenerated() {
		gameData.stageEntityData.RegisterObject(this);
	}

	protected override void Release() {
		base.Release();
		gameData.stageEntityData.UnregisterObject(this);
	}
}