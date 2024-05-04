// File create date:2024/5/4
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
public static class AnimeUtils {

	public const string AnimatorBaseLayer = "BaseLayer";

	public const string AnimatorStateNameIdle = "Idle";
	public const string AnimatorStateNameSummon = "Summon";
	public const string AnimatorStateNameMove = "Move";
	public const string AnimatorStateNameWork = "Work";
	public const string AnimatorStateNameWait = "Wait";
	public const string AnimatorStateNameCarry = "Carry";

	public static int GetBaseStateHash(string sn) {
		return Animator.StringToHash($"{AnimatorBaseLayer}.{sn}");
	}
}