using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RehmanMughal.PostmarkProtocol.Gameplay;
using RehmanMughal.PostmarkProtocol.Levels;

namespace RehmanMughal.PostmarkProtocol.UI
{
    public sealed class RoutePresentation : MonoBehaviour
    {
        public Transform routeRoot;
        public RouteCell cellPrefab;
        public Transform connectorPrefab;
        public Transform parcel;
        public Transform mailbox;
        public Transform mailboxLid;
        public Transform impactStamp;
        public Transform deliveryImprint;
        public Camera gameCamera;
        public Light deskLight;
        public Material dayDesk;
        public Material nightDesk;
        public Renderer deskRenderer;
        public RectTransform boardArea;
        public SceneUI ui;
        public LevelDefinition level { get; private set; }
        public PlanState plan { get; private set; }
        public bool IsRunning { get; private set; }
        private readonly Dictionary<Vector2Int,RouteCell> cells = new Dictionary<Vector2Int,RouteCell>();
        private MaterialPropertyBlock colorBlock;
        private Coroutine simulation;
        private Coroutine impact;
        private Vector3 cameraRest;
        private Rect lastBoardRect;
        private Vector2 lastScreenSize;
        private readonly Vector3[] boardCorners = new Vector3[4];
        private const float CellSize=1.03f;
        private static readonly Color Paper=new Color(.87f,.82f,.68f), Slot=new Color(.74f,.79f,.68f), InkColor=new Color(.63f,.23f,.16f);
        public Vector3 World(Vector2Int cell) => routeRoot.position + new Vector3((cell.x-(level.width-1)*.5f)*CellSize,.04f,(cell.y-(level.height-1)*.5f)*CellSize);

        public void Load(LevelDefinition definition)
        {
            if(simulation!=null) StopCoroutine(simulation);
            if(impact!=null) StopCoroutine(impact);
            IsRunning=false;
            level=definition;
            plan=new PlanState(level);
            foreach(Transform child in routeRoot) Destroy(child.gameObject);
            cells.Clear();
            foreach(var point in level.route.Concat(level.gaps).Distinct())
            {
                var cell=Instantiate(cellPrefab,World(point),Quaternion.identity,routeRoot);
                cell.name=$"Route_{point.x}_{point.y}";
                cell.position=point; cell.owner=this;
                cells.Add(point,cell);
                foreach(var delta in new[]{Vector2Int.right,Vector2Int.up})
                {
                    var next=point+delta;
                    if(!level.route.Contains(point) || !level.route.Contains(next)) continue;
                    var link=Instantiate(connectorPrefab,(World(point)+World(next))*.5f,Quaternion.identity,routeRoot);
                    link.name="RouteLink";
                    link.localScale=delta==Vector2Int.right ? new Vector3(.55f,.015f,.075f) : new Vector3(.075f,.015f,.55f);
                }
            }
            cameraRest=gameCamera.transform.position;
            deskLight.color=level.night ? new Color(.7f,.79f,1f) : new Color(1f,.87f,.66f);
            deskLight.intensity=level.night ? .85f : 1.05f;
            if(deskRenderer!=null) deskRenderer.sharedMaterial=level.night ? nightDesk : dayDesk;
            ResetParcel();
            RefreshStamps();
            FitBoard();
        }
        private void LateUpdate()
        {
            if(boardArea!=null && level!=null && (lastScreenSize!=new Vector2(Screen.width,Screen.height) || lastBoardRect!=boardArea.rect)) FitBoard();
        }
        private void FitBoard()
        {
            if(boardArea==null || level==null || Screen.width==0 || Screen.height==0) return;
            Canvas.ForceUpdateCanvases();
            boardArea.GetWorldCorners(boardCorners);
            var area=Rect.MinMaxRect(boardCorners[0].x/Screen.width,boardCorners[0].y/Screen.height,boardCorners[2].x/Screen.width,boardCorners[2].y/Screen.height);
            if(area.width<=0 || area.height<=0) return;
            var bounds=new Bounds(gameCamera.transform.InverseTransformPoint(World(level.start)),Vector3.zero);
            foreach(var cell in level.route.Concat(level.gaps))
                foreach(var offset in new[]{new Vector3(-.72f,0,-.72f),new Vector3(.72f,.8f,.72f),new Vector3(-.72f,.8f,.72f),new Vector3(.72f,0,-.72f)})
                    bounds.Encapsulate(gameCamera.transform.InverseTransformPoint(World(cell)+offset));
            gameCamera.orthographicSize=Mathf.Max(bounds.size.y/(2*area.height),bounds.size.x/(2*gameCamera.aspect*area.width));
            var desired=new Vector2((area.center.x-.5f)*2*gameCamera.orthographicSize*gameCamera.aspect,(area.center.y-.5f)*2*gameCamera.orthographicSize);
            gameCamera.transform.position+=gameCamera.transform.right*(bounds.center.x-desired.x)+gameCamera.transform.up*(bounds.center.y-desired.y);
            cameraRest=gameCamera.transform.position;
            lastBoardRect=boardArea.rect;lastScreenSize=new Vector2(Screen.width,Screen.height);
        }
        private void ResetParcel()
        {
            parcel.gameObject.SetActive(true);
            parcel.position=World(level.start)+Vector3.up*.21f;
            parcel.rotation=Quaternion.Euler(0,Angle(level.startDirection),0);
            parcel.localScale=Vector3.one*.72f;
            mailbox.position=World(level.destination)+Vector3.up*.14f;
            mailboxLid.localRotation=Quaternion.identity;
            var parcelLabel=parcel.GetComponentInChildren<TextMesh>();
            if(parcelLabel!=null) parcelLabel.text=level.parcel==ParcelType.Fragile ? "!" : level.parcel==ParcelType.Express ? ">>" : level.parcel==ParcelType.Registered ? "R" : "↑";
            if(impactStamp!=null) impactStamp.gameObject.SetActive(false);
            if(deliveryImprint!=null) deliveryImprint.gameObject.SetActive(false);
            var body=parcel.Find("Cotton envelope");
            if(body!=null)
            {
                Color c=level.parcel==ParcelType.Fragile ? new Color(.79f,.54f,.31f) : level.parcel==ParcelType.Express ? new Color(.83f,.45f,.31f) : level.parcel==ParcelType.Registered ? new Color(.49f,.69f,.69f) : new Color(.97f,.93f,.79f);
                Tint(body.GetComponent<Renderer>(),c);
                body.localScale=level.parcel==ParcelType.Fragile ? new Vector3(.81f,.32f,.65f) : new Vector3(.85f,.09f,.58f);
                // Mark sits on top of the body; the Fragile body is much taller, so raise the mark above its top face instead of leaving it buried inside the mesh.
                if(parcelLabel!=null) parcelLabel.transform.localPosition=new Vector3(0,body.localScale.y*.5f+.04f,-.12f);
            }
        }
        private static float Angle(Direction direction) => (int)direction*90;
        private void Tint(Renderer renderer,Color color) { if(colorBlock==null) colorBlock=new MaterialPropertyBlock();colorBlock.SetColor("_Color",color); renderer.SetPropertyBlock(colorBlock); }
        public void RefreshStamps(Vector2Int? impacted=null)
        {
            foreach(var pair in cells)
            {
                var point=pair.Key;
                var cell=pair.Value;
                bool isSlot=level.slots.Any(s=>s.position==point);
                cell.slotBorder.SetActive(isSlot);
                string label=isSlot ? "+" : "";
                var restriction=level.slots.FirstOrDefault(s=>s.position==point);
                if(restriction!=null && restriction.allowedInks.Length==1)
                    label=restriction.allowedInks[0]==Ink.Double ? "+\n×2" : restriction.allowedInks[0]==Ink.Permanent ? "+\nLOCK" : "+";
                Color color=isSlot ? Slot : Paper;
                if(point==level.start || point==level.destination) label="";
                if(level.gaps.Contains(point)) { label="///"; color=new Color(.31f,.28f,.24f); }
                if(level.hazards.Contains(point)) { label="!"; color=new Color(.77f,.51f,.28f); }
                if(level.checkpoints.Contains(point)) { label=isSlot ? label+" ✓" : "CHECK\n✓"; color=new Color(.49f,.68f,.64f); }
                var gate=level.gates.FirstOrDefault(g=>g.position==point);
                if(gate!=null) { label=$"GATE\n{gate.openPhase}/{gate.period}"; color=new Color(.77f,.63f,.42f); }
                foreach(var stamp in plan.Stamps.Where(s=>s.position==point))
                {
                    label=Symbol(stamp.command)+(stamp.ink==Ink.Double ? "\n×2" : stamp.ink==Ink.Permanent ? "\nLOCK" : "");
                    color=stamp.ink==Ink.Permanent ? new Color(.31f,.37f,.39f) : InkColor;
                }
                if(level.checkpoints.Contains(point) && plan.Stamps.Any(s=>s.position==point)) label+="  ✓";
                cell.label.text=label;
                cell.label.characterSize=label.Length>3 ? .034f : .065f;
                cell.label.color=plan.Stamps.Any(s=>s.position==point) || level.gaps.Contains(point) ? new Color(.98f,.94f,.82f) : new Color(.16f,.25f,.24f);
                Tint(cell.surface,color);
            }
            if(impacted.HasValue)
            {
                if(impact!=null) StopCoroutine(impact);
                impact=StartCoroutine(StampImpact(World(impacted.Value)));
            }
        }
        public static string Symbol(Command command)
        {
            switch(command) { case Command.Left:return "↰";case Command.Right:return "↱";case Command.Wait:return "Ⅱ";case Command.Reverse:return "↶";case Command.Fast:return ">>";case Command.Slow:return "~";default:return "^"; }
        }
        public bool TryPoint(Vector2 screen,out Vector2Int cell)
        {
            cell=default;
            if(boardArea!=null && !RectTransformUtility.RectangleContainsScreenPoint(boardArea,screen)) return false;
            // Keep small projected route spaces tappable. Overlapping hit regions resolve to the nearest center.
            float radius=40f*(boardArea!=null ? boardArea.GetComponentInParent<Canvas>().scaleFactor : 1f);
            float nearest=float.MaxValue;
            foreach(var slot in level.slots)
            {
                Vector2 point=gameCamera.WorldToScreenPoint(World(slot.position));
                Vector2 delta=point-screen;
                if(Mathf.Abs(delta.x)<=radius && Mathf.Abs(delta.y)<=radius && delta.sqrMagnitude<nearest)
                { nearest=delta.sqrMagnitude;cell=slot.position; }
            }
            if(nearest<float.MaxValue) return true;
            if(Physics.Raycast(gameCamera.ScreenPointToRay(screen),out var hit,100))
            {
                var tile=hit.collider.GetComponentInParent<RouteCell>();
                if(tile!=null) { cell=tile.position; return true; }
            }
            cell=default; return false;
        }
        public void HighlightAt(Vector2 screen)
        {
            ClearHighlight();
            if(TryPoint(screen,out var point) && cells.TryGetValue(point,out var cell) && ui.CanPlace(point)) cell.transform.localScale=Vector3.one*1.07f;
        }
        public void ClearHighlight() { foreach(var cell in cells.Values) cell.transform.localScale=Vector3.one; }
        public void TutorialSlot(bool highlight)
        {
            if(level==null || level.id!=1) return;
            foreach(var slot in level.slots) if(cells.TryGetValue(slot.position,out var cell)) cell.transform.localScale=Vector3.one*(highlight ? 1.09f : 1);
        }
        private IEnumerator StampImpact(Vector3 position)
        {
            if(impactStamp==null) yield break;
            impactStamp.gameObject.SetActive(true);
            for(float t=0;t<.24f;t+=Time.deltaTime)
            {
                float p=t/.24f;
                impactStamp.position=position+Vector3.up*(.28f+1.3f*Mathf.Abs(p*2-1));
                impactStamp.localScale=new Vector3(.7f,.7f*(1-.15f*Mathf.Sin(p*Mathf.PI)),.7f);
                gameCamera.transform.position=cameraRest+Vector3.up*(Mathf.Sin(p*Mathf.PI*4)*.018f*(1-p));
                yield return null;
            }
            gameCamera.transform.position=cameraRest;
            impactStamp.gameObject.SetActive(false);
        }
        public void Dispatch()
        {
            if(IsRunning) return;
            IsRunning=true;
            ui.RefreshPlanning();
            AudioService.Play("dispatch"); AudioService.Haptic();
            simulation=StartCoroutine(Play(RouteSimulator.Run(level,plan.Stamps)));
        }
        public void Restart(bool clear)
        {
            if(simulation!=null) StopCoroutine(simulation);
            simulation=null; IsRunning=false;
            if(clear) plan.Reset();
            ResetParcel(); RefreshStamps();
            ui.SetText("Lesson",level.lesson);
            ui.SetText("RouteClock","READY TO DISPATCH");
        }
        private IEnumerator Play(SimulationResult result)
        {
            ResetParcel();
            yield return new WaitForSeconds(.22f);
            foreach(var frame in result.frames)
            {
                Vector3 from=parcel.position;
                Vector3 to=World(frame.position)+Vector3.up*.21f;
                Quaternion before=parcel.rotation, after=Quaternion.Euler(0,Angle(frame.direction),0);
                bool jumping=frame.eventName=="Jump" && (to-from).sqrMagnitude>.01f;
                string sound=frame.eventName.ToLowerInvariant();
                if(sound=="move") sound="slide";
                if(sound=="left" || sound=="right") sound="turn";
                if(sound=="slide" || sound=="turn" || sound=="wait" || sound=="reverse" || (sound=="jump" && jumping)) AudioService.Play(sound);
                float duration=frame.duration>0 ? frame.duration : .10f;
                for(float t=0;t<duration;t+=Time.deltaTime)
                {
                    float p=Mathf.SmoothStep(0,1,t/duration);
                    parcel.position=Vector3.Lerp(from,to,p)+(jumping ? Vector3.up*Mathf.Sin(p*Mathf.PI)*.9f : Vector3.zero);
                    parcel.rotation=Quaternion.Slerp(before,after,p);
                    yield return null;
                }
                parcel.position=to; parcel.rotation=after;
                ui.SetText("RouteClock",$"CYCLE {frame.tick:00}  /  {frame.eventName.ToUpperInvariant()}");
                foreach(var gate in level.gates)
                    if(cells.TryGetValue(gate.position,out var cell)) cell.label.text="GATE\n"+(((frame.tick-gate.openPhase)%gate.period+gate.period)%gate.period<gate.openDuration ? "OPEN" : "SHUT");
            }
            if(result.success)
            {
                AudioService.Play("mailbox");
                for(float t=0;t<.45f;t+=Time.deltaTime) { mailboxLid.localRotation=Quaternion.Euler(-95*Mathf.Clamp01(t/.45f),0,0); yield return null; }
                for(float t=0;t<.4f;t+=Time.deltaTime) { parcel.localScale=Vector3.one*.72f*(1-t/.4f); yield return null; }
                parcel.gameObject.SetActive(false);
                AudioService.Play("success"); AudioService.Haptic();
                yield return new WaitForSeconds(.35f);
                yield return StampImpact(routeRoot.position+Vector3.up*.1f);
                if(deliveryImprint!=null) { deliveryImprint.position=routeRoot.position+Vector3.up*.22f; deliveryImprint.gameObject.SetActive(true); }
                AudioService.Play("seal");
                yield return new WaitForSeconds(.55f);
            }
            else
            {
                AudioService.Play("failure"); AudioService.Haptic();
                var from=parcel.position;
                for(float t=0;t<.3f;t+=Time.deltaTime) { parcel.position=from+Vector3.right*Mathf.Sin(t*70)*.07f; yield return null; }
                parcel.position=from;
            }
            IsRunning=false;
            ui.RefreshPlanning();
            ui.DeliveryFinished(result);
        }
    }
}
