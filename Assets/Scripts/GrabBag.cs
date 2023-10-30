using System.Linq;

public class GrabBag<T> where T : class
{
    private T[] _things;
    private T[] _shuffledThings;
    private int _currentIndex = 0;

    public GrabBag(T[] things)
    {
        _things = things;
        ShuffleTransforms();
    }

    public T Grab()
    {
        // If we've gone through all Transforms, shuffle them again.
        if (_currentIndex >= _shuffledThings.Length)
        {
            ShuffleTransforms();
            _currentIndex = 0;
        }

        // Grab the next Transform and move on to the next one for the future.
        return _shuffledThings[_currentIndex++];
    }

    private void ShuffleTransforms()
    {
        // Store the last transform if there was one.
        T lastTransform = (_currentIndex > 0 && _currentIndex == _shuffledThings.Length) ? _shuffledThings[_currentIndex - 1] : null;

        // Shuffle all transforms.
        _shuffledThings = _things.OrderBy(t => UnityEngine.Random.value).ToArray();

        // If the last transform was selected first in the shuffle, swap it with the second one.
        if (_shuffledThings.Length > 1 && _shuffledThings[0] == lastTransform)
        {
            _shuffledThings[0] = _shuffledThings[1];
            _shuffledThings[1] = lastTransform;
        }
    }

}
