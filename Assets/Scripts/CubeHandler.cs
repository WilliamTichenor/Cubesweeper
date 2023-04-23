using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeHandler : MonoBehaviour
{
    int currentx;
    int currenty;
    Image referenceImage;
    bool moving = false;
    bool holding = false;
    Quaternion defaultRot;
    SpriteRenderer[] sides;
    SpriteRenderer[] ghost;
    public Sprite[] colors;
    sbyte moveDir = 0;

    public void PlayerSetup()
    {
        currentx = GameManager.gameGrid.GetLength(1) / 2;
        currenty = GameManager.gameGrid.GetLength(0) / 2;
        transform.position = GameManager.tiles[currenty, currentx].gameObject.transform.position+new Vector3(0,.5f,0);
        defaultRot = Quaternion.Euler(0, 0, 0);
        referenceImage = GameObject.Find("ReferenceImage").GetComponent<Image>();
        sides = new SpriteRenderer[6];
        ghost = new SpriteRenderer[4];
        for (int i=0;i<6;i++)
        {
            sides[i] = GameObject.Find("side" + i).GetComponent<SpriteRenderer>();
            sides[i].sprite = colors[GameManager.cubeSides[i]];
            sides[i].enabled = true;
        }
        for (int i=0;i<4;i++)
        {
            ghost[i] = GameObject.Find("ghost" + (i+1)).GetComponent<SpriteRenderer>();
        }
        UpdateLogic();
    }
    void UpdateLogic()
    {       
        if (moveDir != 0)
        {
            /* Move Dir
             * 1 = Forwards
             * 2 = Right
             * 3 = Backwards
             * 4 = Left
             */
            sbyte[] newSides = new sbyte[6]; 
            if (moveDir == 1)
            {
                newSides[0] = GameManager.cubeSides[1];
                newSides[1] = GameManager.cubeSides[5];
                newSides[2] = GameManager.cubeSides[2];
                newSides[3] = GameManager.cubeSides[0];
                newSides[4] = GameManager.cubeSides[4];
                newSides[5] = GameManager.cubeSides[3];
            }
            else if (moveDir == 2)
            {
                newSides[0] = GameManager.cubeSides[2];
                newSides[1] = GameManager.cubeSides[1];
                newSides[2] = GameManager.cubeSides[5];
                newSides[3] = GameManager.cubeSides[3];
                newSides[4] = GameManager.cubeSides[0];
                newSides[5] = GameManager.cubeSides[4];
            }
            else if (moveDir == 3)
            {
                newSides[0] = GameManager.cubeSides[3];
                newSides[1] = GameManager.cubeSides[0];
                newSides[2] = GameManager.cubeSides[2];
                newSides[3] = GameManager.cubeSides[5];
                newSides[4] = GameManager.cubeSides[4];
                newSides[5] = GameManager.cubeSides[1];
            }
            else if (moveDir == 4)
            {
                newSides[0] = GameManager.cubeSides[4];
                newSides[1] = GameManager.cubeSides[1];
                newSides[2] = GameManager.cubeSides[0];
                newSides[3] = GameManager.cubeSides[3];
                newSides[4] = GameManager.cubeSides[5];
                newSides[5] = GameManager.cubeSides[2];
            }
            GameManager.cubeSides = newSides;
            gameObject.transform.rotation = defaultRot;
            for (int i = 0; i < 6; i++)
            {
                sides[i].sprite = colors[GameManager.cubeSides[i]];
            }
            if (newSides[0] == 1) GameManager.tiles[currenty, currentx].Reveal();
            if (newSides[0] == 2) GameManager.tiles[currenty, currentx].Flag();
        }
        if (GameManager.gameOver) return;
        referenceImage.sprite = GameManager.tiles[currenty, currentx].getSprite();
        for (int i = 0; i < 4; i++)
        {
            ghost[i].sprite = sides[i+1].sprite;
            ghost[i].enabled = true;
        }
        moving = false;
    }
    private void Update()
    {
        if (GameManager.gameOver) return;
        if (!moving)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 && !holding)
            {
                if (currentx + Input.GetAxisRaw("Horizontal") < 0 || currentx + Input.GetAxisRaw("Horizontal") > GameManager.gameGrid.GetLength(1)-1) return;
                moving = true;
                holding = true;
                currentx += (int)Input.GetAxisRaw("Horizontal");
                if (Input.GetAxisRaw("Horizontal") == 1) moveDir = 2;
                else moveDir = 4;
                for (int i = 0; i < 4; i++)
                {
                    ghost[i].enabled = false;
                }
                StartCoroutine(MoveOverSeconds(gameObject, new Vector3(transform.position.x + Input.GetAxisRaw("Horizontal"), transform.position.y, transform.position.z), .1f));
                StartCoroutine(RotateOverSeconds(gameObject,transform.eulerAngles+new Vector3(0,0, Input.GetAxisRaw("Horizontal")*-90),.1f));
            }
            else if (Input.GetAxisRaw("Vertical") != 0 && !holding)
            {
                if (currenty - Input.GetAxisRaw("Vertical") < 0 || currenty - Input.GetAxisRaw("Vertical") > GameManager.gameGrid.GetLength(0) - 1) return;
                moving = true;
                holding = true;
                currenty -= (int)Input.GetAxisRaw("Vertical");
                if (Input.GetAxisRaw("Vertical") == 1) moveDir = 1;
                else moveDir = 3;
                for (int i = 0; i < 4; i++)
                {
                    ghost[i].enabled = false;
                }
                StartCoroutine(MoveOverSeconds(gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z + Input.GetAxisRaw("Vertical")), .1f));
                StartCoroutine(RotateOverSeconds(gameObject, transform.eulerAngles + new Vector3(Input.GetAxisRaw("Vertical") * 90, 0, 0), .1f));                
            }            
        }
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0) holding = false;
        //Debug.Log(Input.GetAxisRaw("Horizontal") + ", " + Input.GetAxisRaw("Vertical")+" : "+holding+" "+moving);
        //Debug.Log(currentx+", "+currenty);
        //Debug.Log(moveDir);
    }
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }
    public IEnumerator RotateOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.rotation.eulerAngles;
        while (elapsedTime < seconds)
        {
            Vector3 euler = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            objectToMove.transform.rotation = Quaternion.Euler(euler);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        UpdateLogic();
    }
}
