

public class Piece {
    private int value;
    private string renderValue; //What the piece would look like if it was rendered
    public int Value {get {return value;}}
    public string RenderValue {get {return renderValue;}}
    public Piece(int value,string renderValue){
        this.value = value;
        this.renderValue = renderValue;
    }
}