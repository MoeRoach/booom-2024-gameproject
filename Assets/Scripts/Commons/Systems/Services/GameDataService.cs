﻿// File create date:2024/4/21
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class GameDataService : IGameService {

	public const string ServiceName = "GameDataService";

	public bool stageGenerated;
	public StageData stageData;
	public StageEntityData stageEntityData;
	
	public void InitService() {
		stageData = new StageData();
	}

	public void KillService() { }
}