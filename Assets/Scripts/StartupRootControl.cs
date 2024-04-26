// File create date:2024/4/18
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class StartupRootControl : BaseGameObject  {
	protected override void OnStart() {
		base.OnStart();
		DelayAction(OnStartupDone, 2f);
	}

	private void OnStartupDone() {
		SceneLoader.Instance.SwitchScene(GlobalConfigs.SceneNameMenu);
	}
}