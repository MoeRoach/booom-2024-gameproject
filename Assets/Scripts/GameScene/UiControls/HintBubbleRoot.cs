﻿// File create date:2024/4/28
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class HintBubbleRoot : BaseUiWidget {

	private Dictionary<int, HintBubble> _pawnBubbles;
	private Dictionary<int, HintBubble> _objectBubbles;
	private Dictionary<int, HintBubble> _facilityBubbles;

	private Dictionary<int, HintBubble> _otherBubbles;

	protected override void LoadMembers() {
		base.LoadMembers();
		_pawnBubbles = new Dictionary<int, HintBubble>();
		_objectBubbles = new Dictionary<int, HintBubble>();
		_facilityBubbles = new Dictionary<int, HintBubble>();
		_otherBubbles = new Dictionary<int, HintBubble>();
	}
}