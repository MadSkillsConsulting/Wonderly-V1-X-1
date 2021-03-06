﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using System.IO;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;


public class FirebaseManager : MonoBehaviour {
	public CloudEndpointsApiManager ceam;
	public InputField email;
  public InputField Password;
	public Text newEmail;
	public InputField currentPasswordInput;
	public InputField newPassword;
	public InputField emailForPasswordReset;
	public string token;
	protected Firebase.Auth.FirebaseAuth auth;
	public FirebaseApp fbApp;
	public Firebase.Storage.FirebaseStorage fbStorage;
	public Firebase.Storage.StorageReference fbStorageRef;
	public Text firstName;
	public Text lastName;

	public GameObject signInScreen;
	public GameObject matchCurrentPasswordPanel;
	public GameObject newPasswordPanel;
	public GameObject newPasswordPanel2;
	public GameObject notMatchingCurrentPasswordNotification;
	public GameObject notMatchingCurrentPasswordNotification2;
	public GameObject invalidPasswordNotification;
	public GameObject passwordChangedNotification;
	public InputField editPassword;
	public InputField editPassword2;

	public GameObject profileIcon1;
	public GameObject profileIcon2;
	public GameObject createIcon1;
	public GameObject createIcon2;
	public GameObject libraryIcon1;
	public GameObject libraryIcon2;

	public GameObject wrongLoginNotification;
	public GameObject wrongSignUpNotification;

	public GameObject passwordResetSuccessNotification;
	public GameObject passwordResetFailNotification;

	public GameObject loadingPanel;

	public bool isLoggedIn;


	public void createNewFirebaseUser()
	{
		
		//make sure password is at least 6 chars long
		if (newPassword.text.Length < 6)
		{
			Debug.Log("password must be at least 6 characters long");
			return;
		}

		string lowerCaseEmail = newEmail.text.ToLower();

		//create new firebase user
		auth.CreateUserWithEmailAndPasswordAsync(lowerCaseEmail, newPassword.text).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				//turn of loading animation
				loadingPanel.SetActive(false);
				wrongSignUpNotification.SetActive(true);
				return;
			}

			// Firebase user has been created.
			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat("Firebase user created successfully: {0} ({1})",
					newUser.DisplayName, newUser.UserId);

			//login
			StartCoroutine("InternalLoginProcess");
		});

	}

    IEnumerator Example()
    {
        print(Time.time);
        yield return new WaitForSeconds(5);
        print(Time.time);
    }

	//only used for signing in directly after creating a new profile
	private IEnumerator InternalLoginProcess()
	{
			//login credentials
			string lowerCaseEmail = newEmail.text.ToLower();
			string p = newPassword.text;
			//this is used for automatically logging you in 
			/*if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
			{
					e = PlayerPrefs.GetString("email", "email");
					p = PlayerPrefs.GetString("password", "password");
			}
			else
			{
					//sets your email and password for later
					PlayerPrefs.SetString("email", email.text);
					PlayerPrefs.SetString("password", Password.text);

			}*/
			//firebase signin
			FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
			auth.SignInWithEmailAndPasswordAsync(lowerCaseEmail, p).ContinueWith(task =>
			{
					if (task.IsCanceled)
					{
							Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
							return;
					}
					if (task.IsFaulted)
					{
							Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
							return;
					}

					Firebase.Auth.FirebaseUser newUser = task.Result;
					Debug.LogFormat("User signed in successfully: {0} ({1})",
							newUser.DisplayName, newUser.UserId);
					GetTokenAfterNewUserCreation(auth);

					Debug.Log("Logging in: " + lowerCaseEmail + " " + p);
					PlayerPrefs.SetString("email", lowerCaseEmail);
					PlayerPrefs.SetString("password", p);
					PlayerPrefs.SetInt("isLoggedIn", 1);
					PlayerPrefs.SetString("fName", firstName.text);
					PlayerPrefs.SetString("lName", lastName.text);


			});
			yield return null;

	}

	//only used for signing in automatically if playerPrefs is filled out
	public IEnumerator InternalLoginProcessAutomatic(string e, string p)
	{
			//this is used for automatically logging you in 
			/*if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
			{
					e = PlayerPrefs.GetString("email", "email");
					p = PlayerPrefs.GetString("password", "password");
			}
			else
			{
					//sets your email and password for later
					PlayerPrefs.SetString("email", email.text);
					PlayerPrefs.SetString("password", Password.text);

			}*/
			//firebase signin
			FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
			auth.SignInWithEmailAndPasswordAsync(e, p).ContinueWith(task =>
			{
					if (task.IsCanceled)
					{
							Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
							return;
					}
					if (task.IsFaulted)
					{
							Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
							return;
					}

					Firebase.Auth.FirebaseUser newUser = task.Result;
					Debug.LogFormat("User signed in successfully: {0} ({1})",
							newUser.DisplayName, newUser.UserId);
					GetToken(auth);

					Debug.Log("Logging in: " + e + " " + p);

			});
			yield return null;
	}

	public void StartLoginProcess()
	{
			//login credentials
			string p = Password.text;
			string lowerCaseEmail = email.text.ToLower();
			//this is used for automatically logging you in 
			/*if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
			{
					e = PlayerPrefs.GetString("email", "email");
					p = PlayerPrefs.GetString("password", "password");
			}
			else
			{
					//sets your email and password for later
					PlayerPrefs.SetString("email", email.text);
					PlayerPrefs.SetString("password", Password.text);

			}*/
			//firebase signin
			FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
			auth.SignInWithEmailAndPasswordAsync(lowerCaseEmail, p).ContinueWith(task =>
			{
					Debug.Log("Attempted logging in: " + lowerCaseEmail + " " + p);
					if (task.IsCanceled)
					{
							Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
							return;
					}
					if (task.IsFaulted)
					{
							Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
							Debug.Log("Password or email incorrect");
							//turn off loading animattion
							loadingPanel.SetActive(false);
							wrongLoginNotification.SetActive(true);
							return;
					}

					Debug.Log("after task fault check");

					//turn off loading animation
					loadingPanel.SetActive(false);
					signInScreen.SetActive(false);
					Firebase.Auth.FirebaseUser newUser = task.Result;
					Debug.LogFormat("User signed in successfully: {0} ({1})",
							newUser.DisplayName, newUser.UserId);
					GetToken(auth);

					Debug.Log("Logging in: " + lowerCaseEmail + " " + p);
					PlayerPrefs.SetString("email", lowerCaseEmail);
					PlayerPrefs.SetString("password", p);
					PlayerPrefs.SetInt("isLoggedIn", 1);
			});
	}


	public void GetToken(FirebaseAuth auth)
	{
			FirebaseUser user = auth.CurrentUser;

			user.TokenAsync(true).ContinueWith(task =>
			{
					if (task.IsCanceled)
					{
							Debug.LogError("TokenAsync was canceled.");
							return;
					}

					if (task.IsFaulted)
					{
							Debug.LogError("TokenAsync encountered an error: " + task.Exception);
							Debug.Log("Password or email incorrect");
							wrongLoginNotification.SetActive(true);
							return;
					}

					token = task.Result;
					Debug.Log(token);
					isLoggedIn = true;

					profileIcon1.SetActive(false);
					profileIcon2.SetActive(true);
					libraryIcon1.SetActive(false);
					libraryIcon2.SetActive(true);
					createIcon1.SetActive(false);
					createIcon2.SetActive(true);
		});
	}

	public void GetTokenAfterNewUserCreation(FirebaseAuth auth)
	{
			FirebaseUser user = auth.CurrentUser;

			user.TokenAsync(true).ContinueWith(task =>
			{
					if (task.IsCanceled)
					{
							Debug.LogError("TokenAsync was canceled.");
							return;
					}

					if (task.IsFaulted)
					{
							Debug.LogError("TokenAsync encountered an error: " + task.Exception);
							Debug.Log("Password or email incorrect");
							wrongLoginNotification.SetActive(true);
							return;
					}

					token = task.Result;
					Debug.Log(token);
					isLoggedIn = true;
					ceam.startProfileCreate();

					profileIcon1.SetActive(false);
					profileIcon2.SetActive(true);
					libraryIcon1.SetActive(false);
					libraryIcon2.SetActive(true);
					createIcon1.SetActive(false);
					createIcon2.SetActive(true);
		});
	}

	// Use this for initialization
	void Start () 
	{
		isLoggedIn= false;
        //firebase init
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
					Debug.Log("Firebase OK!");
			}
			else
			{
					UnityEngine.Debug.LogError(System.String.Format(
						"Could not resolve all Firebase dependencies: {0}", dependencyStatus));
					// Firebase Unity SDK is not safe to use here.
			}
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://aliceone-221018.firebaseio.com");

				//set class variable to auth instance
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;       
				//set class variable to firebase app instance
				fbApp = FirebaseApp.DefaultInstance;
				//set class variable to firebase storage instance
			  fbStorage = Firebase.Storage.FirebaseStorage.DefaultInstance;
				fbStorageRef = fbStorage.GetReferenceFromUrl("gs://aliceone-221018.appspot.com");

		});
	}

	public void signOutFirebase()
	{
		auth.SignOut();
		isLoggedIn = false;
		PlayerPrefs.SetInt("isLoggedIn", 0);
		PlayerPrefs.SetString("email", "");
		PlayerPrefs.SetString("password", "");
	}

	public void sendPasswordResetEmail()
	{
		string emailAddress = emailForPasswordReset.text;
			auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task => {
				if (task.IsCanceled) {
					Debug.LogError("SendPasswordResetEmailAsync was canceled.");
					loadingPanel.SetActive(false);
					passwordResetFailNotification.SetActive(true);
					emailForPasswordReset.text = "";
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
					loadingPanel.SetActive(false);
					passwordResetFailNotification.SetActive(true);
					emailForPasswordReset.text = "";
					return;
				}

				loadingPanel.SetActive(false);
				passwordResetSuccessNotification.SetActive(true);
				emailForPasswordReset.text = "";
				Debug.Log("Password reset email sent successfully.");
			});
	}

	public void checkMatchingPassword()
	{
		if(currentPasswordInput.text == PlayerPrefs.GetString("password"))
		{
			//deactivate current panel
			matchCurrentPasswordPanel.SetActive(false);
			//deactivate bad password error notification if it has popped up
			notMatchingCurrentPasswordNotification.SetActive(false);
			//activate next panel
			newPasswordPanel.SetActive(true);
			currentPasswordInput.text = "";
		}
		else
		{
			//show error message of not matching passwords
			notMatchingCurrentPasswordNotification.SetActive(true);

		}
	}

	public void checkValidPassword()
	{
		if (editPassword.text.Length >= 6)
		{
			//deactivate current screen
			newPasswordPanel.SetActive(false);
			//activate next screen
			newPasswordPanel2.SetActive(true);
			invalidPasswordNotification.SetActive(false);
		}
		else
		{
			//show error
			invalidPasswordNotification.SetActive(true);
		}
	}

	public void checkMatchingNewPassword()
	{
		if(editPassword.text == editPassword2.text)
		{
			//call firebase method to change password to newPasswordInput2
			Debug.Log("passwords match");
			changeUserPassword();

		}
		else
		{
			//show error message of not matching passwords
			notMatchingCurrentPasswordNotification2.SetActive(true);

		}
	}

	public void changeUserPassword()
	{
		Debug.Log("in change user password");
		Firebase.Auth.FirebaseUser user = auth.CurrentUser;
		string newPassword = editPassword2.text;
		if (user != null) {
			user.UpdatePasswordAsync(newPassword).ContinueWith(task => {
				if (task.IsCanceled) {
					Debug.LogError("UpdatePasswordAsync was canceled.");
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError("UpdatePasswordAsync encountered an error: " + task.Exception);
					return;
				}

				//activate notifaction 
				passwordChangedNotification.SetActive(true);
				//set new password in player prefs for auto log in
				PlayerPrefs.SetString("password", newPassword);
				Debug.Log("Password updated successfully.");
				//reset password input text
				editPassword.text = "";
				editPassword2.text = "";
			});
		}
		else
		{
			Debug.Log("firebase user is null");
		}
	}

	public void clearPasswordInputs()
	{
		currentPasswordInput.text = "";
		editPassword.text = "";
		editPassword2.text = "";
	}
}
