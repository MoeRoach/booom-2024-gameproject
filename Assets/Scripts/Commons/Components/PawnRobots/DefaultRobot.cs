// File create date:2024/4/29
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class DefaultRobot : BasePawnRobot {

	private readonly CountdownTimer _thinkTimer;

	public DefaultRobot(BasePawnController src) : base(src) {
		_thinkTimer = new CountdownTimer(1f, true);
	}

	protected override bool CanDecide() {
		_thinkTimer.UpdateTimer(Time.deltaTime);
		return _thinkTimer.Done;
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