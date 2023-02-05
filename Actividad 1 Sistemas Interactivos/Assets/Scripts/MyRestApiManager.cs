using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyRestApiManager : MonoBehaviour
{
    //main api source variables
    string myServerApiPath = "https://my-json-server.typicode.com/LunarBullet/Actividad-1---Sistemas-Interactivos/users/";
    string rickAndMortyApiSource = "https://rickandmortyapi.com/api/character/";

    //user and deck related variables
    [SerializeField] int myDeckUserIDCounter = 1; //allows us to easily check from inspector which user we are currently checking out
    public int[] DeckCards;
    [SerializeField] List<RawImage> myRawImageList;

    public void GetMyPlayerInfoOnClick() => StartCoroutine(GetMyPlayerInfoCorrutine());
    public void ChangeMyDeckUserNumber(int number) => myDeckUserIDCounter = number;

    //gets the player info from our source, starts all related corrutines for capture and download of character images
    IEnumerator GetMyPlayerInfoCorrutine()
    {
        UnityWebRequest myUnityWebRequest = UnityWebRequest.Get(myServerApiPath + myDeckUserIDCounter);
        Debug.Log(rickAndMortyApiSource + myDeckUserIDCounter);

        yield return myUnityWebRequest.Send();

        if (myUnityWebRequest.isNetworkError)
        {
            Debug.Log("Houston, we've got a problem!");
            Debug.Log("\nNetwork error:" + myUnityWebRequest.error);
        }
        else
        {
            if (myUnityWebRequest.responseCode == 200) //if everything is OK
            {
                Debug.Log("Alles Gut!");

                MyUserData myUser = JsonUtility.FromJson<MyUserData>(myUnityWebRequest.downloadHandler.text);
                Debug.Log("Name:" + myUser.name);
                int counter = 0;

                foreach (var user in myUser.deck)
                {
                    StartCoroutine(GetCharacterCorrutine(user, counter));
                    yield return new WaitForSeconds(0.2f);
                    counter++;

                }
            }
            else
            {
                Debug.Log("Houston, we've got a problem, yet again!"); //something went wrong
                Debug.Log("Status: " + myUnityWebRequest.responseCode);
            }

                byte[] myResults = myUnityWebRequest.downloadHandler.data;
        }
    }

    //gets user and starts download corrutine
    IEnumerator GetCharacterCorrutine(int characterID, int ImageIndex)
    {
        UnityWebRequest myUnityWebRequest = UnityWebRequest.Get(rickAndMortyApiSource + characterID);
        Debug.Log(rickAndMortyApiSource + characterID);
        yield return myUnityWebRequest.Send();

        if (myUnityWebRequest.isNetworkError)
        {
            Debug.Log("Houston, we've got a problem, yet again!");
            Debug.Log("NETWORK ERROR:" + myUnityWebRequest.error);
        }
        else
        {
            if (myUnityWebRequest.responseCode == 200) //all good saul good man!
            {
                Debug.Log("We are doing OK!");

                MyCharacter character = JsonUtility.FromJson<MyCharacter>(myUnityWebRequest.downloadHandler.text);
                StartCoroutine(DownloadImageCorrutine(character.image, ImageIndex));
            }
            else Debug.Log("Houston, we've got a problem, yet again!"); //something went wrong

            byte[] results = myUnityWebRequest.downloadHandler.data;
        }
    }

    //gets, sets and downloads the image of the user
    IEnumerator DownloadImageCorrutine(string imageUrl, int ImageIndex)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) Debug.Log("Houston, we've got a problem, again: " + request.error);
        else
        {
            Debug.Log("All is OK!");
            myRawImageList[ImageIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}

[System.Serializable]
public class MyCharacterList
{
    public MyCharacterListInfo info;
    public List<MyCharacter> results;
}

[System.Serializable]
public class MyCharacter
{
    public int id;
    public string name;
    public string specie;
    public string image;
}
public class MyCharacterImage
{
    public string image;
}

public class MyUserData
{
    public int id;
    public string name;
    public int[] deck;
}

[System.Serializable]
public class MyCharacterListInfo
{
    public int count;
    public int pages;
    public string prev;
    public string next;
}