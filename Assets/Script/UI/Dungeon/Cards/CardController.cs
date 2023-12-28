using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(EventTrigger))]
public class CardController : MonoBehaviour
{
	public static Vector2 ScreenResolution = new Vector2(1600, 900);

[Header("Components")]
	public RectTransform rectTransform;

	public GameObject CardFace;
	public Image CardHighlight;
	public GameObject CardBack;
	#region Card Highlight
	public void SetHighlight(bool value)
    {
		if (CardHighlight != null) CardHighlight.gameObject.SetActive(value);
    }
	#endregion
	#region Card Orders
	public class CardMoveOrder
	{
		public Vector2 Pos = Vector2.zero;
		public Vector3 Scale = Vector3.zero;
		public Vector3 Rotation = Vector3.zero;
		public int Frames;
		public int currentFrame = 0;
		public bool forced = false;
		public bool deathOnFinish = false;

		public Vector2 origPos = Vector2.zero;
		public Vector3 origScale =Vector3.zero;
		public Vector3 origRotation = Vector3.zero;

		public CardMoveOrder(CardController Owner, float nDuration, Vector3 center, float vScale, Vector3 vRot, bool force)
		{
			Frames = Mathf.CeilToInt( nDuration * 30);
			Pos = center;

			Scale = Vector3.one *  vScale;

			forced = force;
			Rotation = vRot;

			origPos = Owner.transform.localPosition;
			origRotation = Owner.gameObject.transform.localRotation.eulerAngles;
			origScale = Owner.rectTransform.localScale;
		}
	}
	CardMoveOrder Order = null;
	[HideInInspector] public bool inMotion = false;
	public CardMoveOrder GetOrder()
	{
		return Order;
	}
	public bool isIdle()
	{
		return Order == null;
	}
	public void GiveOrder(CardMoveOrder nOrder)
	{
		if (!(Order != null && Order.deathOnFinish))
		{
			if (Order == null || nOrder == null || nOrder.forced)
			{
				Order = nOrder;
				if (Order.Frames <= 0)
				{
					Snap();
				}
			}
		}
	}
	public void ClearOrder()
	{
		Order = null;

	}
	#endregion
	#region EventTrigger
	[HideInInspector] public int zIndex;
	public Vector2 pointerPosition = Vector2.zero;
	public void InitEventTrigger()
	{

		EventTrigger evT = GetComponent<EventTrigger>();
		if (Application.platform == RuntimePlatform.Android)
		{
			EventTrigger.Entry tap = new EventTrigger.Entry();
			tap.eventID = EventTriggerType.PointerClick;
			tap.callback.AddListener((data) =>
			{
					if (PlayerHandController.main.SelectedCard == this)
						PlayerHandController.main.DeselectCard();
					else
					PlayerHandController.main.SelectCard(this);
				
			});

			evT.triggers.Add(tap);
		}

else {

					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerEnter;
					entry.callback.AddListener((data) =>
					{
						if (!inMotion && isIdle())
							PlayerHandController.main.SelectCard(this);
					});

					EventTrigger.Entry exit = new EventTrigger.Entry();
					exit.eventID = EventTriggerType.PointerExit;
					exit.callback.AddListener((data) =>
					{
						if (!inMotion && isIdle())
							PlayerHandController.main.DeselectCard();
					});

					evT.triggers.Add(entry);
					evT.triggers.Add(exit);
		}

		EventTrigger.Entry begindrag = new EventTrigger.Entry();
		begindrag.eventID = EventTriggerType.BeginDrag;
		begindrag.callback.AddListener((data) =>
		{
			inMotion = true;
			SetHighlight(true);
			if (CastingAssistant.main!=null)
            {
				CastingAssistant.main.gameObject.SetActive(true);
				CastingAssistant.main.AssignSpell(assignedAbility);

			}
		});

		EventTrigger.Entry drag = new EventTrigger.Entry();
		drag.eventID = EventTriggerType.Drag;
		drag.callback.AddListener((data) => {


			/*	Vector2 motionCard = transform.parent.localPosition;
				GiveOrder(new CardMoveOrder(this, .2f,  //TODO define
					new Vector3(
						Input.mousePosition.x - Screen.width / 2f,
						Input.mousePosition.y - Screen.height / 2f,
						0
						) / transform.parent.lossyScale.x - (Vector3)motionCard,
					1, Vector3.zero, true));*/

			pointerPosition = ((PointerEventData)data).position;


				//PlayerHandController.main.isCheckingCards = false;
				PlayerHandController.main.isDraggingCard = true;

			if (CastingAssistant.main != null)
			{
				CastingAssistant.main.AdjustDestination( pointerPosition);

			}

		});


		EventTrigger.Entry enddrag = new EventTrigger.Entry();

		enddrag.eventID = EventTriggerType.EndDrag;
		enddrag.callback.AddListener((data) => {
			if (!inMotion)
				return;
			//if (GameController.main.GetCurrentPhase() == GameController.GamePhase.PlayPhase)
			{
				//TODO plays
				/* if (transform.localPosition.y>100 && myCardData.CanCast())
				{
					GamePieceController TUM = GameController.main.GamePieceUnderCursor;
					if (TUM == null)
					{
						if (myCardData.CanCastNoTarget())
						{
							myCardData.CastOnPiece(null);
							Die();
						}
					}
					else
					{
						if (myCardData.CanCastOnPiece(TUM))
						{
							myCardData.CastOnPiece(TUM);
							Die();
						}
					}
				}
				if (myCardData.CanBeSacrificed())
				{
					float width = 200 * transform.localScale.x;
					Vector2 delta = (Vector2)transform.localPosition - GameUIController.main.GetAltarOrigin();
					if (delta.sqrMagnitude < 200 * 200)
					{
						myCardData.OnSacrificed();
						Die();
					}

				}*/
				if (CastingAssistant.main != null)
				{
					CastingAssistant.main.gameObject.SetActive(false);
				}
				SetHighlight(false);
				if (pointerPosition.y > 300)
				{
						Play();
				}
			}
			inMotion = false;
			//PlayerHandController.main.isCheckingCards = false;
			PlayerHandController.main.isDraggingCard = false;
			PlayerHandController.main.ResetCardPosition(true,true);
		});
		evT.triggers.Add(drag);
		evT.triggers.Add(begindrag);
		evT.triggers.Add(enddrag);
	}
	#endregion
	#region Enable/Disable
	public void Shrink()
	{
		Discard(new CardMoveOrder(
			this,
			CardDefines.CardSpeedNormal,
			transform.localPosition, 0, transform.rotation.eulerAngles,
			true
		));
	}

	public void Drop()
	{
		Discard(new CardMoveOrder(
			this,
			CardDefines.CardSpeedNormal,
			 Vector3.down * 500f, 1, transform.rotation.eulerAngles,
			true
		)) ;
	}   public void Shuffle()
	{

		Discard(new CardMoveOrder(
			this,
			CardDefines.CardSpeedNormal,
			PlayerHandController.main.deck.position, 1, transform.rotation.eulerAngles,
			true
		));
	}
	public void Discard(CardMoveOrder dOrder)
	{
		Order = dOrder;
		Order.forced = true;
		Order.deathOnFinish = true;
	}
	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}
	private void Start()
	{
		InitEventTrigger();
	}
	private void OnEnable()
	{
		transform.SetAsFirstSibling();
	}
	private void OnDisable()
	{
		ClearAbilityData();
		ClearOrder();
	}
	#endregion
	#region Face
	[Header("Face")]
	public Image abilityIcon;
	public TextMeshProUGUI abilityCost;
	public TextMeshProUGUI abilityName;
	public TextMeshProUGUI abilityDescription;

	void DrawFace()
	{
		if (abilityName != null)
		{
			abilityName.text = assignedAbility.original.InternalName;
		}
		if (abilityDescription != null)
		{
			abilityDescription.text = assignedAbility.original.AbilityDesc;
		}
		if (abilityCost != null)
		{
			abilityCost.text = Mathf.Ceil(assignedAbility.GetAbilityCost()) + "";
		}
		if (abilityIcon != null)
		{
			abilityIcon.sprite = assignedAbility.original.sprite;
		}
		if (assignedAbility.caster is Hero hero)
		{
			if (abilityIcon != null)
			{
				abilityIcon.color = hero.classComponent.AssignedClass.heroColor;
			}
			if (CardHighlight != null)
			{
				CardHighlight.color = hero.classComponent.AssignedClass.heroColor;
			}
		}
	}
        #endregion
        #region Card Ability
        PropertyCard assignedAbility;
	public PropertyCard GetAssignedAbility()
	{
		return assignedAbility;
	}
	public void LoadAbilityData(Hero caster, AbilitySO abilityData)
	{
        PropertyCard ability = new PropertyCard(caster, abilityData);
		LoadAbilityData(ability);
	}
	public void LoadAbilityData(PropertyCard ability)
	{
		assignedAbility = ability;
		DrawFace();
	}
	public void ClearAbilityData()
	{
		assignedAbility = null;
	}

	#endregion
	#region Play
	public void Play()
	{
		PlayerHandController.main.PlayCard(this,pointerPosition);
	}

    #endregion
    #region Frame By Frame
    public virtual void Update()
	{
		if (Order != null)
		{
			Move();
			Flip();
		}
	}

	public void Move()
	{
		Vector3 vRotation = gameObject.transform.localRotation.eulerAngles;
		Vector2 vPosition = transform.localPosition;
		Vector3 vScale = rectTransform.localScale;

		if (Order.Pos != null)
		{
			if (vPosition != Order.Pos)
			{
				vPosition += (Order.Pos - Order.origPos) / Order.Frames;
			}

			if (vScale != Order.Scale)
			{
				vScale += (Order.Scale - Order.origScale) / Order.Frames;
			}
		}

		if (Order.Rotation != null)
		{
			if (vRotation != Order.Rotation)
			{
				vRotation += 
					new Vector3(
					Mathf.DeltaAngle(Order.origRotation.x,Order.Rotation.x),
					Mathf.DeltaAngle(Order.origRotation.y , Order.Rotation.y ),
					Mathf.DeltaAngle(Order.origRotation.z, Order.Rotation.z)
				) / Order.Frames;
			}
		}

		Order.currentFrame++;

		if (Order.currentFrame > Order.Frames)
		{
			if (Order.deathOnFinish)
			{
				gameObject.SetActive(false);
			}
			Snap();
			return;
		}

		transform.localPosition = new Vector3(vPosition.x, vPosition.y);
		gameObject.transform.rotation = Quaternion.Euler(vRotation);
		rectTransform.localScale = vScale;
	}
	public void Snap()
	{
		if (Order != null)
		{
			transform.localPosition = new Vector2(Order.Pos.x, Order.Pos.y);
			gameObject.transform.rotation = Quaternion.Euler(Order.Rotation);
			rectTransform.localScale = new Vector3(Order.Scale.x, Order.Scale.y, 1);
		}
		ClearOrder();
	}
	bool faceUp = true;
	public virtual void Flip()
	{
		Vector3 vRot = gameObject.transform.rotation.eulerAngles;

		int Rot = 0;
		if (vRot.x > 90 && vRot.x < 270)
		{
			Rot++;
		}
		if (vRot.y > 90 && vRot.y < 270)
		{
			Rot++;
		}
		faceUp = Mathf.Round(Rot / 2) == (float)Rot / 2f;
		UpdateFace();

	}
	void UpdateFace()
    {
		UpdateFace(faceUp);
    }
	void UpdateFace(bool va)
	{
		CardBack.SetActive(!va);
		CardFace.SetActive(va );
	}
	#endregion
}
