﻿// File create date:2024/4/29
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
public class DefaultPawn : BasePawnController  {
	// Script Code
	protected override BasePawnRobot CreateRobot() {
		return new DefaultRobot(this);
	}
}