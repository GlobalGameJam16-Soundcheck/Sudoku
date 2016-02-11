using UnityEngine;
using System.Collections;

public class pieceBehavior : MonoBehaviour {

	public int player;
	public int iDirLen; //vertical length in the grid, check from +0 to +iDirLen and -0 to -iDirLen
	public int jDirLen; //horizontal length in the grid, check from +0 to +jDirLen and -0 to -jDirLen
	public int iDirLenStar;//for stars, they're special, have a plus sign
	public int jDirLenStar; 
	public Material mat;
	public Material matStar;


}
