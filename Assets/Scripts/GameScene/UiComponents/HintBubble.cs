﻿// File create date:2024/4/28
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
using UnityEngine.UI;

// Created By Yu.Liu
public class HintBubble : BaseUiWidget {

	public static class HintConfigs {
		public const string HintTagThink = "HintThink";
	}

	private Image _hintIcon;
	private string _hintTag;
	
	public int Id { get; private set; }

	protected override void LoadViews() {
		base.LoadViews();
		_hintIcon = FindComponent<Image>("HintIcon");
	}

	public void SetupId(int id) {
		Id = id;
	}

	public void SetupHint(string ht) {
		_hintTag = ht;
	}

	public override void UpdateViews() {
		base.UpdateViews();
		// 根据Tag设置图标
	}
}