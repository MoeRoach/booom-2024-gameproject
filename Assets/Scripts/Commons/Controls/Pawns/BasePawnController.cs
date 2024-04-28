﻿// File create date:2024/4/18
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

	public virtual async UniTask AsyncMove(Vector3 target, float spd, CancellationTokenSource cts) {
		var start = transform.position;
		var delta = target - start;
		var invt = spd / delta.magnitude;
		var timer = 0f;
		while (timer < 1f) {
			timer += Time.deltaTime * invt;
			transform.position = Vector3.Lerp(start, target, timer);
			await UniTask.Yield(cts.Token);
		}
	}

	public virtual async UniTask AsyncCollect(GameObject target, float time, CancellationTokenSource cts) {
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}

	public virtual async UniTask AsyncCarryon(GameObject target, CancellationTokenSource cts) {
		// 把东西带上
		await UniTask.Yield(cts.Token);
	}
	
	public virtual async UniTask AsyncPutdown(GameObject target, CancellationTokenSource cts) {
		// 把东西放下
		await UniTask.Yield(cts.Token);
	}
	
	public virtual async UniTask AsyncProduce(GameObject target, float time, CancellationTokenSource cts) {
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}
	
	public virtual async UniTask AsyncAttack(GameObject target, CancellationTokenSource cts) {
		// 攻击目标，播放攻击动画，等待事件处理和动画完结，然后再返回
		await UniTask.Yield(cts.Token);
	}
	
	public virtual async UniTask AsyncThink(GameObject target, float time, CancellationTokenSource cts) {
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}
	
	public virtual async UniTask AsyncHappy(GameObject target, float time, CancellationTokenSource cts) {
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}
	
	public virtual async UniTask AsyncFreeze(GameObject target, float time, CancellationTokenSource cts) {
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}
	
	public virtual async UniTask AsyncThunder(GameObject target, float time, CancellationTokenSource cts) {
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}
	
	public virtual async UniTask AsyncMeteor(GameObject target, float time, CancellationTokenSource cts) {
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}

	protected override void Release() {
		base.Release();
		gameData.stageEntityData.UnregisterPawn(this);
	}
}