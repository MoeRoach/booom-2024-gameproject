// File create date:2024/4/22
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class GameData {
	// Script Code
}

public class PawnOperation {
	
	public int pid;
	public string op;
	public bool priority;
	public Dictionary<string, string> args;

	public string this[string key] {
		get => args.TryGetElement(key);
		set => args[key] = value;
	}

	public PawnOperation(int id) {
		pid = id;
		args = new Dictionary<string, string>();
	}

	public void PutInteger(string key, int val) {
		args[key] = val.ToString();
	}

	public int GetInteger(string key) {
		if (!args.ContainsKey(key)) return int.MinValue;
		var chk = int.TryParse(args[key], out var ret);
		return chk ? ret : int.MinValue;
	}

	public void PutFloat(string key, float val) {
		args[key] = val.ToString("f2");
	}

	public float GetFloat(string key) {
		if (!args.ContainsKey(key)) return float.NaN;
		var chk = float.TryParse(args[key], out var ret);
		return chk ? ret : float.NaN;
	}

	public void PutExtra<T>(string key, T val) {
		args[key] = JsonConvert.SerializeObject(val);
	}

	public T GetExtra<T>(string key) {
		return !args.ContainsKey(key) ? default : JsonConvert.DeserializeObject<T>(args[key]);
	}
}

public static class PawnOperateConfigs {

	public const string OpMoveto = "MoveTo";
	public const string OpCollect = "Collect";
	public const string OpCarryon = "CarryOn";
	public const string OpPutdown = "PutDown";
	public const string OpProduce = "Produce";
	public const string OpAttack = "Attack";
	
	public const string OpThink = "Think";
	public const string OpHappy = "Happy";
	
	public const string OpFreeze = "Freeze";
	public const string OpThunder = "Thunder";
	public const string OpMeteor = "Meteor";

	public const string ExtraKeyIdentifier = "Identifier";
	public const string ExtraKeyTarget = "Target";
	public const string ExtraKeyDirection = "Direction";
}