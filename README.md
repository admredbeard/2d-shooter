# 2d-shooter

## How to work with GIT
* branchname should be named after feature/issue along with issue number EX: mycoolability/3 (where 3 is issue number)
* Create own branch to work on `git checkout -b "branchname"`
* Checkout branch to work on `git checkout "branchname"`
* Work on branch and GTFO gamescene
* Pushing: make sure u in ur branch push by: `git push origin "branchname"`

## How to work on github
* create pull-request for ur branch into (dev-branch) (needs to be changed top left corner)
* someone could approve but whatever
* press "files changed"
* press "review changes"
* make sure correct merges are made
* click merge
* press merge pull request
* voila devbranch should be uptodate with ur branch
* checkout devbranch on git and pull by `git pull origin devbranch` (to make sure someone elses changes are locally at u now)

## API Calls

```C#
public void Move(int unitId, float angle)
    {
        //Unit must be on our team
    }

    public void LookAtDirection(int unitId, float angle)
    {

    }

    public void AngleBetween(int unitId, int targetId)
    {
        //target must be within our vision
    }

    public List<GameObject> SenseNearby(int unitId)
    {

        List<GameObject> nearbyUnits = new List<GameObject>();

        return nearbyUnits;
    }

    public List<GameObject> SenseNearbyByTeam(int unitId, int team)
    {
        List<GameObject> nearbyUnits = new List<GameObject>();

        return nearbyUnits;
    }

    public List<GameObject> GetTeammates(int unitId)
    {
        List<GameObject> teammates = new List<GameObject>();

        return teammates;
    }

    public Vector2 GetWorldPosition(int unitId)
    {
        //Check if it is in our team, then its ok to see it. Otherwise we have
        //to check if it is within our vision range.

        Vector2 position = new Vector2();
        return position;
    }

    public Vector2Int GetGridPos(int unitId)
    {
        //Check if it is in our team, then its ok to see it. Otherwise we have
        //to check if it is within our vision range.

        Vector2Int position = new Vector2Int();
        return position;
    }

    public bool TargetInSight(int unitId, int targetId)
    {
        return false;
    }

    public bool WorldPositionInSight(int unitId, Vector2 worldPosition)
    {
        return false;
    }

    public bool GridPositionInSight(int unitId, Vector2Int worldPosition)
    {
        return false;
    }

    public void Attack(int unitId)
    {

    }

    public void SwapWeapon(int unitId, Weapon newWeapon)
    {

    }

    public void ReloadWeapon(int unitId)
    {

    }

    public float GetAmmunition(int unitId)
    {
        // We can not check enemy ammunition
        return 0f;
    }

    public Weapon GetWeapon(int unitId)
    {
        return Weapon.Knife;
    }

    public float GetHealth(int unitId)
    {
        return 0f;
    }

    public bool IsGridPositionTraversable(Vector2Int gridPosition){
        return false;
    }

    public bool IsWorldPositionTraversable(Vector2 worldPosition){
        return false;
    }

    public bool [,] GetMap(){
        bool [,] map = new bool [100,100];
        return map;
    }

    public float GetDistanceToUnit(int unitId, int targetId){
        return 0f;
    }

    public float GetDistanceToGridPosition(int unitId, Vector2Int gridPosition){
        return 0f;
    }

    public float GetDistanceToWorldPosition(int unitId, Vector2 worldPosition){
        return 0f;
    }
```
