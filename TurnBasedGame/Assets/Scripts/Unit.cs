﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool selected;
    GameMaster gm;
    public int tileSpeed;
    public bool hasMoved;

    public float moveSpeed;

    public int playerNumber;

    public int attackRange;
    List<Unit> enemisInRange = new List<Unit>();
    public bool hasAttacked;

    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    public DamageIcon damageIcon;

    public GameObject weaponIcon;

    private void Start() {
        gm = FindObjectOfType<GameMaster>();
    }

    private void OnMouseDown() {

        ResetWeaponIcons();

        if(selected == true)
        {
            selected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();
        }
        else
        {
            if(playerNumber == gm.playerTurn)
            {
                if(gm.selectedUnit != null)
                {
                    gm.selectedUnit.selected = false;
                }

                selected= true;
                gm.selectedUnit = this;

                gm.ResetTiles();
                GetEnemies();
                GetWalkableTile();
            }
            
        }
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition),0.15f);
        Unit unit = col.GetComponent<Unit>();
        if(gm.selectedUnit != null)
        {
            if(gm.selectedUnit.enemisInRange.Contains(unit) && gm.selectedUnit.hasAttacked == false)
            {
                gm.selectedUnit.Attack(unit);
            }
        }
    }

    public void ResetWeaponIcons()
    {
        foreach( Unit unit in FindObjectsOfType<Unit>())
        {
            unit.weaponIcon.SetActive(false);
        }
    }


    void Attack(Unit enemy)
    {
        hasAttacked = true;

        int enemyDamage= attackDamage - enemy.armor;
        int myDamage = enemy.defenseDamage - armor;

        if(enemyDamage >= 1)
        {
            DamageIcon instance =Instantiate(damageIcon,enemy.transform.position,Quaternion.identity);
            instance.Setup(enemyDamage);
            enemy.health -= enemyDamage;

        }
        if(myDamage>=1)
        {
            DamageIcon instance =Instantiate(damageIcon,transform.position,Quaternion.identity);
            instance.Setup(myDamage);
            health -= myDamage;
        }
        if(enemy.health <=0)
        {
            Destroy(enemy.gameObject);
            GetWalkableTile();
        }
        if(health <= 0)
        {
            gm.ResetTiles();
            Destroy(this.gameObject);
        }

    }


    void GetEnemies()
    {
        enemisInRange.Clear();

        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            if((Mathf.Abs(transform.position.x - unit.transform.position.x) + Mathf.Abs(transform.position.y -unit.transform.position.y))<= attackRange)
            {
                if(unit.playerNumber != gm.playerTurn && hasAttacked == false)
                {
                    enemisInRange.Add(unit);
                    unit.weaponIcon.SetActive(true);
                }
            }
        }
    }

    void GetWalkableTile()
    {
        if(hasMoved == true)
        {
            return;
        }

        foreach(Tile tile in FindObjectsOfType<Tile>())
        {
            if((Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y -tile.transform.position.y))<= tileSpeed)
            {
                if(tile.IsClear() == true)
                {
                    tile.Highlight();
                }
            }
        }
    }

    public void Move(Vector2 tilePos)
    {
        StartCoroutine(StartMovement(tilePos));
    }

    IEnumerator StartMovement(Vector2 tilePos)
    {
        while(transform.position.x != tilePos.x)
        {
            transform.position = Vector2.MoveTowards(transform.position,new Vector2(tilePos.x,transform.position.y),moveSpeed*Time.deltaTime);
            yield return null;
        }
        while(transform.position.y != tilePos.y)
        {
            transform.position = Vector2.MoveTowards(transform.position,new Vector2(transform.position.x,tilePos.y),moveSpeed*Time.deltaTime);
            yield return null;
        }
        hasMoved = true;
        ResetWeaponIcons();
        GetEnemies();
    }
}