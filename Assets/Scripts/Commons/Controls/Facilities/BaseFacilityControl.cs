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

	protected Vector3 facilityAreaOffset;
	protected bool isGenerate;
	
	public int Id { get; private set; }
	public Square Coord { get; protected set; }
	protected HashSet<Square> area;
	public IEnumerable<Square> Area => area;

	protected override void OnMachineAwake() {
		base.OnMachineAwake();
		area = new HashSet<Square>();
		avatarSprite = FindComponent<SpriteRenderer>("Avatar");
		facilityAreaOffset = avatarSprite.transform.localPosition;
		animatorMachine = avatarSprite.GetComponent<AnimatorStateMachine>();
	}
	
	protected override void OnMachineStart() {
		base.OnMachineStart();
		gameData = GameSystem.Instance.GameData;
	}
	
	public void SetupIdentifier(int id) {
		Id = id;
	}
	
	public void SetupCoord(Square coord) {
		Coord = coord;
		transform.position = MapUtils.SquareToWorld(Coord);
	}

	public void SetupArea(IEnumerable<Square> a) {
		area.Clear();
		area.UnionWith(a);
	}

	protected virtual void InitArea() {
		area.Add(Square.Zero);
		// 可以重载时处理偏移
	}

	protected override void OnLazyStart() {
		if (isGenerate) return;
		OnFacilityGenerated();
	}

	public void NotifyGenerate() {
		isGenerate = true;
		NextFrameAction(OnFacilityGenerated);
	}

	protected virtual void OnFacilityGenerated() {
		Coord ??= MapUtils.WorldToSquare(transform.position);
		InitArea();
		OnAvatarUpdate();
		gameData.stageEntityData.RegisterFacility(this);
	}

	protected virtual void OnAvatarUpdate() {
		avatarSprite.transform.localPosition = facilityAreaOffset;
	}

	protected override void Release() {
		base.Release();
		gameData.stageEntityData.UnregisterFacility(this);
	}
}