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
	
	protected Vector3 objectAreaOffset;
	protected bool isGenerate;
	
	public int Id { get; private set; }
	public Square Coord { get; protected set; }
	//public Square Area { get; protected set; }
	protected HashSet<Square> area;
	public IEnumerable<Square> Area => area;

	protected override void OnMachineAwake() {
		base.OnMachineAwake();
		area = new HashSet<Square>();
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
		OnObjectGenerated();
	}

	public void NotifyGenerate() {
		isGenerate = true;
		NextFrameAction(OnObjectGenerated);
	}

	public void Pickup(GameObject src) {
		transform.SetParent(src.transform);
		OnPickup(src);
	}
	
	protected virtual void OnPickup(GameObject src) { }

	public void Putdown(GameObject src, Square coord) {
		transform.SetParent(ObjectRootControl.Instance.transform);
		OnPutdown(src, coord);
	}

	protected virtual void OnPutdown(GameObject src, Square coord) {
		SetupCoord(coord);
	}

	public virtual void DestroyIt() {
		// 可以重载为异步
		Destroy(gameObject);
	}

	protected virtual void OnObjectGenerated() {
		Coord ??= MapUtils.WorldToSquare(transform.position);
		InitArea();
		OnAvatarUpdate();
		gameData.stageEntityData.RegisterObject(this);
	}
	
	protected virtual void OnAvatarUpdate() {
		avatarSprite.transform.localPosition = objectAreaOffset;
	}

	protected override void Release() {
		base.Release();
		gameData.stageEntityData.UnregisterObject(this);
	}
}