﻿// File create date:2024/4/28
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class WorkerRobot : BasePawnRobot  {
	
	private readonly CountdownTimer _thinkTimer;

	public WorkerRobot(BasePawnController src) : base(src) {
		_thinkTimer = new CountdownTimer(1f, true);
	}
	public override void Initialize() {
		moveSpeed = 4f;
		base.Initialize();
	}

	protected override bool CanDecide() {
		_thinkTimer.UpdateTimer(Time.deltaTime);
		return _thinkTimer.Done;
	}

	protected override void EnterWaiting() {
		owner.BackToIdle();
	}

	protected override void ExitWaiting() {
		_thinkTimer.ResetTimer();
	}

	protected override void Decide() {
		var target = dataService.stageData.RandomTileCoord();
		var moveOp = CreateOperation(PawnOperateConfigs.OpMoveto);
		moveOp.PutExtra(PawnOperateConfigs.ExtraKeyCoordinate, target);
		RequestOperation(moveOp);
	}
}