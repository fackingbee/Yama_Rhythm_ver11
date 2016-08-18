﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class NavigationViewController : ViewController {						// ViewControllerクラスを継承

	private Stack<ViewController> stackedViews = new Stack<ViewController>();	// ビューの階層を保持するスタック
	private ViewController        currentView  = null;							// 現在のビューを保持

	[SerializeField] private Text   titleLabel;									// ナビゲーションバーのタイトルを表示するテキスト
	[SerializeField] private Button backButton;									// ナビゲーションバーのバックボタン
	[SerializeField] private Text   backButtonLabel;							// バックボタンのテキスト


	// インスタンスのロード時に呼ばれる
	void Awake() {
		
		// バックボタンのイベントリスナーを設定する
		backButton.onClick.AddListener(OnPressBackButton);

		// 最初はバックボタンを非表示にする
		backButton.gameObject.SetActive(false);

	}


	// バックボタンが押されたときに呼ばれるメソッド
	public void OnPressBackButton() {
		// 1つ前の階層のビューに戻る
		Pop();
	}


	// ユーザーのインタラクションを有効/無効にするメソッド
	private void EnableInteraction(bool isEnabled) {
		GetComponent<CanvasGroup>().blocksRaycasts = isEnabled;
	}



	// 次の階層のビューへ遷移する処理をおこなうメソッド
	public void Push(ViewController newView) {
		if(currentView == null){
			
			// 最初のビューはアニメーションなしで表示する
			newView.gameObject.SetActive(true);

			currentView = newView;

			return;
		}

		// アニメーションの最中はユーザーのインタラクションを無効にする
		EnableInteraction(false);

		// 現在表示されているビューを画面左外に移動する
		ViewController lastView = currentView;

		stackedViews.Push(lastView);

		Vector2 lastViewPos = lastView.CachedRectTransform.anchoredPosition;

		lastViewPos.x = -this.CachedRectTransform.rect.width;

		lastView.CachedRectTransform.MoveTo(
			lastViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, ()=>{
				// 移動が終わったらビューを非アクティブにする
				lastView.gameObject.SetActive(false);
			});


		// 新しいビューを画面右外から中央に移動する
		newView.gameObject.SetActive(true);

		Vector2 newViewPos = newView.CachedRectTransform.anchoredPosition;

		newView.CachedRectTransform.anchoredPosition = new Vector2(this.CachedRectTransform.rect.width, newViewPos.y);

		newViewPos.x = 0.0f;

		newView.CachedRectTransform.MoveTo(
			newViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, ()=>{
				// 移動が終わったらユーザーのインタラクションを有効にする
				EnableInteraction(true);
			});

		// 新しいビューを現在のビューとして保持して、ナビゲーションバーのタイトルを変更する
		currentView     = newView;
		titleLabel.text = newView.Title;

		// バックボタンのラベルを変更する
		backButtonLabel.text = lastView.Title;

		// バックボタンをアクティブにする
		backButton.gameObject.SetActive(true);
	}


	// 前の階層のビューへ戻る処理をおこなうメソッド
	public void Pop() {
		if(stackedViews.Count < 1) {
			// 前の階層のビューがないので何もしない
			return;
		}

		// アニメーションの最中はユーザーのインタラクションを無効にする
		EnableInteraction(false);

		// 現在表示されているビューを画面右外に移動する
		ViewController lastView = currentView;

		Vector2 lastViewPos     = lastView.CachedRectTransform.anchoredPosition;

		lastViewPos.x           = this.CachedRectTransform.rect.width;

		lastView.CachedRectTransform.MoveTo(
			lastViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, ()=>{
				// 移動が終わったらビューを非アクティブにする
				lastView.gameObject.SetActive(false);
			});

		// 前の階層のビューをスタックから戻し、画面左外から中央に移動する
		ViewController poppedView = stackedViews.Pop();

		poppedView.gameObject.SetActive(true);

		Vector2 poppedViewPos = poppedView.CachedRectTransform.anchoredPosition;

		poppedViewPos.x = 0.0f;

		poppedView.CachedRectTransform.MoveTo(
			poppedViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, ()=>{
				// 移動が終わったらユーザーのインタラクションを有効にする
				EnableInteraction(true);
			});

		// スタックから戻したビューを現在のビューとして保持して、ナビゲーションバーのタイトルを変更する
		currentView     = poppedView;
		titleLabel.text = poppedView.Title;

		// 前の階層のビューがある場合、バックボタンのラベルを変更してアクティブにする
		if(stackedViews.Count >= 1){
			backButtonLabel.text = stackedViews.Peek().Title;
			backButton.gameObject.SetActive(true);
		} else {
			backButton.gameObject.SetActive(false);
		}
	}
}