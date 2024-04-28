﻿// File create date:2024/4/28
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
public class PawnWorker : BasePawnController  {
	// Script Code
	protected override BasePawnRobot CreateRobot() {
		return new WorkerRobot(this);
	}
}