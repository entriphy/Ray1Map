﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.Input;

namespace R1Engine
{
    public class Editor : MonoBehaviour {
        //Settings
        public int autoScrollMargin = 60;
        public float autoScrollSpeed = 5;
        //Colours for selections
        public Color colorSelect, colorNew, colorDelete;
        //References
        public LevelMainController lvlController;
        public LevelEventController lvlEventController;
        public LevelTilemapController lvlTilemapController;
        public SelectSquare tileSelectSquare;
        public Tilemap previewTilemap;
        //Reference to UI buttons
        public Button[] modeButtons;
        //Reference to everything that should be visible in each mode
        public GameObject[] modeContents;
        //Reference to layer buttons
        public GameObject layerTypes;
        public GameObject layerEvents;

        //Current tile under the mouse
        public Common_Tile mouseTile;

        bool selecting;
        bool dragging;
        [HideInInspector]
        public bool scrolling;
        [HideInInspector]
        public EditorCam cam;

        //Different edit modes to choose from
        public enum EditMode { Tiles, Collisions, Events, Links }
        //Current edit mode
        [HideInInspector]
        public EditMode currentMode;
        //Current type
        [HideInInspector]
        public TileCollisionType currentType;

        // Current map index
        public int currentMap = 0;

        //Selected tiles
        public Common_Tile[,] selection;

        private readonly List<Common_Tile> TempPrevTileHistory = new List<Common_Tile>();
        private readonly List<Common_Tile> TempTileHistory = new List<Common_Tile>();

        public void SetEditMode(int mode) {
            // Set
            currentMode = (EditMode)mode;
            //Change button visibility
            for(int i=0; i<modeButtons.Length; i++) {
                ColorBlock b = modeButtons[i].colors;
                if ((int)currentMode == i) {
                    b.normalColor = new Color(1,1,1);
                    modeContents[i].SetActive(true);
                }
                else {
                    b.normalColor = new Color(0.5f, 0.5f, 0.5f);
                    modeContents[i].SetActive(false);
                }
                modeButtons[i].colors = b;
            }

            //What to show/hide with each mode
            layerTypes.SetActive(currentMode == EditMode.Collisions);
            layerEvents.SetActive(currentMode == EditMode.Events || currentMode == EditMode.Links);
            lvlEventController.ToggleLinks(currentMode == EditMode.Links);

            tileSelectSquare.gameObject.SetActive(currentMode == EditMode.Tiles || currentMode == EditMode.Collisions);
            previewTilemap.gameObject.SetActive(currentMode == EditMode.Tiles);

            if (currentMode != EditMode.Tiles) {
                if (lvlTilemapController.focusedOnTemplate) {
                    lvlTilemapController.ShowHideTemplate();
                }
            }
        }

        public void SetCurrentType(int type) {
            currentType = (TileCollisionType)type;
        }

        /*
        void SetAlpha(float visAlpha, float colAlpha) {
            foreach (var tm in lvlController.controllerTilemap.Tilemaps)
                if (tm.name == "TilemapTypes")
                    tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, colAlpha);
                else tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, visAlpha);
        }

        public EditMode mode { get => _mode; set {
                if (_mode == value) return;
                _mode = value;

                // Set display transparencies for different edit modes
                switch (value) {
                    case EditMode.Tiles:
                    case EditMode.Events:
                    case EditMode.Links:
                        SetAlpha(1, 0); break;
                    case EditMode.Collisions:
                        SetAlpha(0.35f, 0.9f); break;
                }

                tileSelectSquare.Clear();
            } }
        */

        void Awake() {
            cam = Camera.main.GetComponent<EditorCam>();
        }

        void Start() {
            //Default to events
            SetEditMode(2);
        }

        public void ClearSelection() {
            selection = null;
            tileSelectSquare.Clear();
            if (currentMode == EditMode.Tiles)
                previewTilemap.ClearAllTiles();
        }

        void Update() {

            //Tile editing
            if (currentMode == EditMode.Tiles || currentMode == EditMode.Collisions) {
                // Get the tile under the mouse
                Vector3 mousePositionTile = lvlController.controllerTilemap.MouseToTileCoords(mousePosition);
                mouseTile = lvlController.controllerTilemap.GetTileAtPos((int)mousePositionTile.x, -(int)mousePositionTile.y);

                // =============== SELECTION SQUARE ===============

                // Escape clears selection info
                if (GetKeyDown(KeyCode.Escape)) {
                    ClearSelection();
                }                

                // Left click begins drag and assigns the starting corner of the selection square
                if (!dragging && mouseTile != null) {
                    if (GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                        tileSelectSquare.SetStartCorner(mousePositionTile.x, -mousePositionTile.y);
                        dragging = true;
                        selecting = !GetKey(KeyCode.LeftControl);
                        //Clear old preview tilemap
                        if (currentMode == EditMode.Tiles)
                            previewTilemap.ClearAllTiles();
                    }
                }

                if (dragging) {
                    // During drag, set the end corner
                    var endX = Mathf.Clamp(mousePositionTile.x, 0, lvlController.currentLevel.Maps[currentMap].Width - 1);
                    var endY = Mathf.Clamp(-mousePositionTile.y, 0, lvlController.currentLevel.Maps[currentMap].Height - 1);
                    if (selecting) {
                        tileSelectSquare.color = colorSelect;
                        tileSelectSquare.SetEndCorner(endX, endY);
                    } else {

                        tileSelectSquare.color = colorNew;
                        tileSelectSquare.SetEndCorner(endX, endY);

                        if (currentMode == EditMode.Tiles) {
                            //Change preview position
                            previewTilemap.transform.position = new Vector3((int)tileSelectSquare.XStart, -(int)tileSelectSquare.YStart);
                            //Expand the preview tiles
                            if (selection != null) {
                                previewTilemap.ClearAllTiles();
                                int xi = 0;
                                int yi = 0;
                                for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                    for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                        previewTilemap.SetTile(new Vector3Int(x - (int)tileSelectSquare.XStart, y - (int)tileSelectSquare.YStart, 0), Controller.obj.levelController.currentLevel.Maps[currentMap].TileSet[0].Tiles[selection[xi, yi].TileSetGraphicIndex]);
                                        xi++;
                                        if (xi >= selection.GetLength(0))
                                            xi = 0;
                                    }
                                    xi = 0;
                                    yi++;
                                    if (yi >= selection.GetLength(1))
                                        yi = 0;
                                }
                            }
                        }
                    }

                    // Auto scroll if dragging near the screen margin and not manually moving the camera
                    if (GetMouseButton(1)) {
                        float scr = Camera.main.orthographicSize * cam.friction * autoScrollSpeed * Time.deltaTime;
                        bool inMarginLeft = mousePosition.x < autoScrollMargin;
                        bool inMarginRight = mousePosition.x > Screen.width - autoScrollMargin;
                        bool inMarginTop = mousePosition.y < autoScrollMargin;
                        bool inMarginBottom = mousePosition.y > Screen.height - autoScrollMargin;
                        bool inMargin = inMarginLeft || inMarginRight || inMarginTop || inMarginBottom;

                        if (inMarginLeft) cam.vel.x -= scr;
                        if (inMarginRight) cam.vel.x += scr;
                        if (inMarginTop) cam.vel.y -= scr;
                        if (inMarginBottom) cam.vel.y += scr;

                        if (inMargin)
                            scrolling = true;
                    }
                }
                else {
                    scrolling = false;
                    //Move preview with mouse if not dragging
                    previewTilemap.transform.position = mousePositionTile;
                }

                // Reset camera friction from the drag's override if moved manually
                if (GetMouseButtonDown(1))
                    cam.friction = cam.fricStart;

                if (dragging) {
                    if (currentMode == EditMode.Tiles) {
                        // If dragging and selecting mouse up, record the selection
                        if (selecting && !GetMouseButton(0)) {
                            dragging = false;
                            //Create array for selected area
                            selection = new Common_Tile[(int)(tileSelectSquare.XEnd - tileSelectSquare.XStart) + 1, (int)(tileSelectSquare.YEnd - tileSelectSquare.YStart) + 1];
                            int xi = 0;
                            int yi = 0;
                            //Save the selected area
                            for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {
                                    var t = lvlController.controllerTilemap.GetTileAtPos(x, y);
                                    selection[xi, yi] = t;
                                    //Also fill out preview tilemap
                                    if (currentMode == EditMode.Tiles)
                                        previewTilemap.SetTile(new Vector3Int(xi, yi, 0), Controller.obj.levelController.currentLevel.Maps[currentMap].TileSet[0].Tiles[t.TileSetGraphicIndex]);
                                    xi++;
                                }
                                xi = 0;
                                yi++;
                            }
                        }
                        // If dragging and painting mouse up, set the selection
                        if (!selecting && GetMouseButtonUp(0)) {
                            dragging = false;
                            if (selection != null) {
                                if (!lvlTilemapController.focusedOnTemplate) {
                                    int xi = 0;
                                    int yi = 0;
                                    //"Paste" the selection
                                    for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                        for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) {

                                            var t = Controller.obj.levelController.currentLevel.Maps[currentMap].Tiles.FindItem(item => item.XPosition == x && item.YPosition == y);
                                            TempPrevTileHistory.Add(t.CloneTile());

                                            var tile = lvlController.controllerTilemap.SetTileAtPos(x, y, selection[xi, yi], t);
                                            TempTileHistory.Add(tile.CloneTile());

                                            xi++;
                                            if (xi >= selection.GetLength(0))
                                                xi = 0;
                                        }
                                        xi = 0;
                                        yi++;
                                        if (yi >= selection.GetLength(1))
                                            yi = 0;
                                    }
                                }
                                //Cut the preview back to the original size since it's been expanded
                                if (currentMode == EditMode.Tiles) {
                                    previewTilemap.ClearAllTiles();
                                    //Save the selected area
                                    for (int y = 0; y <= selection.GetLength(1) - 1; y++) {
                                        for (int x = 0; x <= selection.GetLength(0) - 1; x++) {
                                            previewTilemap.SetTile(new Vector3Int(x, y, 0), Controller.obj.levelController.currentLevel.Maps[currentMap].TileSet[0].Tiles[selection[x, y].TileSetGraphicIndex]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (currentMode == EditMode.Collisions) {
                        //If dragging and selecting mouse up; clear types
                        if (selecting && !GetMouseButton(0)) {
                            dragging = false;
                            //"Paste" the selection
                            for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) 
                                {
                                    var t = Controller.obj.levelController.currentLevel.Maps[currentMap].Tiles.FindItem(item => item.XPosition == x && item.YPosition == y);
                                    TempPrevTileHistory.Add(t.CloneTile());

                                    var tile = lvlController.controllerTilemap.SetTypeAtPos(x, y, 0, t);
                                    TempTileHistory.Add(tile.CloneTile());
                                }
                            }
                        }
                        //Fill with selected type
                        if (!selecting && GetMouseButtonUp(0)) {
                            dragging = false;
                            //"Paste" the selection
                            for (int y = (int)tileSelectSquare.YStart; y <= tileSelectSquare.YEnd; y++) {
                                for (int x = (int)tileSelectSquare.XStart; x <= tileSelectSquare.XEnd; x++) 
                                {
                                    var t = Controller.obj.levelController.currentLevel.Maps[currentMap].Tiles.FindItem(item => item.XPosition == x && item.YPosition == y);
                                    TempPrevTileHistory.Add(t.CloneTile());

                                    var tile = lvlController.controllerTilemap.SetTypeAtPos(x, y, currentType, t);
                                    TempTileHistory.Add(tile.CloneTile());
                                }
                            }
                        }
                    }
                }

                //Stamp selection with ctrl+v
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V) && !dragging) {
                    if (selection != null && !lvlTilemapController.focusedOnTemplate) {
                        int xi = 0;
                        int yi = 0;
                        int w = Controller.obj.levelController.currentLevel.Maps[currentMap].Width;
                        int h = Controller.obj.levelController.currentLevel.Maps[currentMap].Height;
                        int my = -(int)mousePositionTile.y;
                        int mx = (int)mousePositionTile.x;
                        //"Paste" the selection
                        for (int y = my; y <= my+selection.GetLength(1)-1; y++) {
                            for (int x = mx; x <= mx+selection.GetLength(0)-1; x++) {
                                
                                if (x>=0 && y>=0 && x<w && y<h)
                                {
                                    var t = Controller.obj.levelController.currentLevel.Maps[currentMap].Tiles.FindItem(item => item.XPosition == x && item.YPosition == y);
                                    TempPrevTileHistory.Add(t.CloneTile());

                                    var tile = lvlController.controllerTilemap.SetTileAtPos(x, y, selection[xi, yi], t);
                                    TempTileHistory.Add(tile.CloneTile());
                                }

                                xi++;
                            }
                            xi = 0;
                            yi++;
                        }
                    }
                }

                // Check if the editor actions should be undone/redone
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        // Redo
                        Controller.obj.levelController.History.Redo();
                    else
                        // Undo
                        Controller.obj.levelController.History.Undo();
                }

                // Add to history if any tiles were added
                if (TempTileHistory.Any())
                {
                    // Add to history
                    Controller.obj.levelController.History.AddToHistory(new EditorHistoryEntry<Ray1MapEditorHistoryItem>(new Ray1MapEditorHistoryItem(TempPrevTileHistory.ToArray()), new Ray1MapEditorHistoryItem(TempTileHistory.ToArray())));

                    // Clear temp lists
                    TempTileHistory.Clear();
                    TempPrevTileHistory.Clear();
                }
            }
        }
    }
}