using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHandler : MonoBehaviour
{
    int x;
    int y;
    sbyte gridVal;
    sbyte visible;
    SpriteRenderer sr;
    bool tripped;
    public void Setup(int r, int c)
    {
        tripped = false;
        name = r + "," + c;
        x = c;
        y = r;
        gridVal = GameManager.gameGrid[r, c];
        visible = GameManager.visibleGrid[r, c];
        sr = gameObject.GetComponent<SpriteRenderer>();       
        sr.enabled = true;
    }
    public void Reveal()
    {
        if (visible == 2 || visible == 1) return;
        if (gridVal == -1)
        {
            tripped = true;
            sr.sprite = GameManager.sprites[12];
            Debug.Log("Game Over!");
            GameObject.Find("GameManagerObject").GetComponent<GameManager>().GameOver();
        }
        else
        {
            sr.sprite = GameManager.sprites[gridVal];
            GameManager.revealedTiles++;
        }
        visible = 1;
        if (gridVal==0)
        {
            if (x>0&&y>0&&GameManager.tiles[y - 1, x - 1].visible==0)
            {
                GameManager.tiles[y - 1, x - 1].Reveal();
            }
            if (y > 0 && GameManager.tiles[y-1, x].visible == 0)
            {
                GameManager.tiles[y-1, x].Reveal();
            }
            if (x < GameManager.tiles.GetLength(1)-1 && y > 0 && GameManager.tiles[y-1, x+ 1].visible == 0)
            {
                GameManager.tiles[y-1, x+1].Reveal();
            }
            if (x > 0 && GameManager.tiles[y, x-1].visible == 0)
            {
                GameManager.tiles[y, x-1].Reveal();
            }
            if (x < GameManager.tiles.GetLength(1)-1 && GameManager.tiles[y, x+1].visible == 0)
            {
                GameManager.tiles[y, x+1].Reveal();
            }
            if (x > 0 && y < GameManager.tiles.GetLength(0)-1 && GameManager.tiles[y+1, x-1].visible == 0)
            {
                GameManager.tiles[y+1, x-1].Reveal();
            }
            if (y < GameManager.tiles.GetLength(0)-1 && GameManager.tiles[y+1, x].visible == 0)
            {
                GameManager.tiles[y+1, x].Reveal();
            }
            if (x < GameManager.tiles.GetLength(1)-1 && y < GameManager.tiles.GetLength(0)-1 && GameManager.tiles[y + 1, x + 1].visible == 0)
            {
                GameManager.tiles[y + 1, x + 1].Reveal();
            }
        }
    }
    public void Flag()
    {
        if (visible==0)
        {
            //Debug.Log("Flagged!");
            visible = 2;
            sr.sprite = GameManager.sprites[10];
        }
        else if (visible == 2)
        {
            //Debug.Log("Unflagged!");
            visible = 0;
            sr.sprite = GameManager.sprites[11];
        }
    }
    public Sprite getSprite()
    {
        return sr.sprite;
    }
    public void RevealMine()
    {
        if (tripped) return;
        if (gridVal == -1)
        {
            sr.sprite = GameManager.sprites[9];
        }
        if (gridVal!=-1 && visible==2) sr.sprite = GameManager.sprites[13];
    }
}
