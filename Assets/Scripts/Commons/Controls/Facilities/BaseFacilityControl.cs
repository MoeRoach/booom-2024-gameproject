// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public abstract class BaseFacilityControl : HierarchyStateMachine  {

	protected enum FacilityState {
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
		YieldAction(OnFacilityGenerated);
	}

	protected virtual void OnFacilityGenerated() {
		gameData.stageEntityData.RegisterFacility(this);
	}

	protected override void Release() {
		base.Release();
		gameData.stageEntityData.UnregisterFacility(this);
	}
}