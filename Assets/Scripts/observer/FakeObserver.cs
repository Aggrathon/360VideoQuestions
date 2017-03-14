using UnityEngine;
using System.Collections;

public static class FakeObserver
{
	private static Vector3 RandDir()
	{
		return new Vector3(Random.value * 120f - 60f, Random.value * 180f - 90f, Random.value * 90f - 45f);
	}

	public static IEnumerator ObserveExample(ObserverManager om)
	{
		yield return null;
		om.HandleMessage("idle");
		yield return new WaitForSeconds(1);
		om.HandleMessage(new MessageData("Example", "", Vector3.zero, 0).ToString());
		yield return new WaitForSeconds(1);
		Permutation perm = new Permutation(9, true);
		om.HandleMessage(new MessageData("Example", "color1", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(1);
		perm.Randomize();
		om.HandleMessage(new MessageData("Example", "color2", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(3);
		perm.Randomize();
		om.HandleMessage(new MessageData("Example", "color1", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(1);

		perm.Randomize();
		om.HandleMessage(new MessageData("Example", "restart", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(1);
		perm.Randomize();
		om.HandleMessage(new MessageData("Example", "restart", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(1);
		perm.Randomize();
		om.HandleMessage(new MessageData("Example", "restart", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(1);
		om.HandleMessage(new MessageData("Example", "color1", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(0.1f);
		perm.Randomize();
		om.HandleMessage(new MessageData("Example", "restart", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(1);
		perm.Randomize();
		om.HandleMessage(new MessageData("Example", "color1", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(0.1f);
		perm.Randomize();
		om.HandleMessage(new MessageData("Example", "restart", RandDir(), perm.Number).ToString());
		yield return new WaitForSeconds(0.5f);

		om.HandleMessage("idle");
		yield return new WaitForSeconds(1);
		om.HandleMessage("disconnect");
	}
}