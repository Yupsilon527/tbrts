using UnityEngine;

public class OrderDisplay : MonoBehaviour
{
   /*

	public void ClearOrderDisplay()
	{
		if (OrderDisplay != null) {
			foreach (GameObject Zim in OrderDisplay) {
				Destroy (Zim);
			}
			OrderDisplay = null;
		}
	}

	public void DoOrderDisplay()
	{
		ClearOrderDisplay ();
		ResetColorTiles ();
		if (SelectedArmy != null) {
			OrderDisplay = new List<GameObject> ();
			List<entityTile> TempTiles = new List<entityTile> ();
			SelectedArmy.recalculateEntirePath ();

			if (SelectedArmy.OrderList.Count > 0) {

				int Mov = 0;

				foreach (entityOrder Panty in SelectedArmy.OrderList) {

					if (Panty.Path != null) {
						foreach (entityTile Stockings in Panty.Path) {


							if (!TempTiles.Contains (Stockings)) {
								TempTiles.Add (Stockings);
								GameObject Zim = new GameObject ();
								(game.GetIndex<List<GameObject>> ("Visuals")).Add (Zim);
								Zim.transform.position = new Vector3 (
									(Stockings.Pos.x + 1 / 2f) * Game.iTileSize - 1 / 2f,
									0 - (Stockings.Pos.y) * Game.iTileSize, 
									Game.ShadowLevel - 0.2f);

								Zim.transform.localScale = new Vector2 (3f / 30, 3f / 30);

								string Mytext = (Mov + 1) + "";
								Mov+=Pathfinder.GetMoveCost(SelectedArmy.GetMyMovementType(),Stockings.iElevation,Stockings.isRoad);

								float Loc = Mathf.Floor ((Mov + SelectedArmy.Movement) / SelectedArmy.GetMyMovement ());
								
								if (Loc>0)
								{
									Mytext += " (" + Loc + ")";
									//Mov = 0;
								}
								if (Panty.Path.IndexOf (Stockings) == Panty.Path.Count - 1) {
									Mytext += "\n" + game.language.OrderNames [(int)Panty.OrderID];
								}

								Zim.AddComponent<TextMesh> ();
								Zim.GetComponent<TextMesh> ().text = Mytext;
								Zim.GetComponent<TextMesh> ().fontSize = 30;
								Zim.GetComponent<TextMesh> ().offsetZ = 2;
								Zim.GetComponent<TextMesh> ().color = Color.white;
								Zim.GetComponent<TextMesh> ().anchor = TextAnchor.MiddleCenter;
								Zim.GetComponent<TextMesh> ().alignment = TextAlignment.Center;
								Zim.GetComponent<TextMesh> ().font = Resources.GetBuiltinResource (typeof(Font), "Arial.ttf") as Font;

								OrderDisplay.Add (Zim);
							}
						}

					} else if (Panty.OrderID == entityOrder.ID.Rest) {
						GameObject Zim = new GameObject ();
						(game.GetIndex<List<GameObject>> ("Visuals")).Add (Zim);
						Zim.transform.position = new Vector3 (
							SelectedArmy.Pos.x * Game.iTileSize - 1 / 2f,
							0 - SelectedArmy.Pos.y * Game.iTileSize, 
							Game.ShadowLevel - 0.2f);

						Zim.transform.localScale = new Vector2 (3f / 30, 3f / 30);

						string Mytext = game.language.OrderNames [(int)Panty.OrderID];

						Zim.AddComponent<TextMesh> ();
						Zim.GetComponent<TextMesh> ().text = Mytext;
						Zim.GetComponent<TextMesh> ().fontSize = 30;
						Zim.GetComponent<TextMesh> ().offsetZ = 2;
						Zim.GetComponent<TextMesh> ().anchor = TextAnchor.MiddleCenter;
						Zim.GetComponent<TextMesh> ().alignment = TextAlignment.Center;
						Zim.GetComponent<TextMesh> ().font = Resources.GetBuiltinResource (typeof(Font), "Arial.ttf") as Font;

						OrderDisplay.Add (Zim);
					}
				}
			}
		}	
	}*/
}
