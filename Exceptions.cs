public class PointZeroException : Exception { // For throwing if user tries to use a space input of 0x0
    public PointZeroException()
    : base("Invalid space 0 x 0 detected.") {}
}

public class SpaceTakenException : Exception { // For throwing if player enters a space with a piece already on it
    public SpaceTakenException()
    : base("Space is already taken by a piece. Please try again.") {}
}