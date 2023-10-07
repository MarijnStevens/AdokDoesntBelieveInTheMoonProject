
using System.Diagnostics;

public class Benchmarky
{
  readonly Stopwatch _framewatch = new();

  int _frames = 0;
  long _fps = 0;
  double _averageFPS = 0;
  int _previousFrameIndex = 0;
  readonly long[] _previousFrames = new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

  public double AverageFPS { get { return _averageFPS; } }

  public long FPS { get { return _fps; } }

  public Benchmarky()
  {
    _framewatch.Start();
  }

  public void RecordFps()
  {
    _frames++;
    var elapsedMilliseconds = _framewatch.ElapsedMilliseconds;

    if (elapsedMilliseconds >= 1000)
    {
      var initAverage = false;
      if (_fps == 0)
      {
        initAverage = true;
      }

      _fps = _frames * 1000 / elapsedMilliseconds;

      if (initAverage)
      {
        for (var i = 0; i < _previousFrames.Length; ++i)
        {
          _previousFrames[i] = _fps;
        }
      }

      _previousFrameIndex++;

      if (_previousFrameIndex > _previousFrames.Length - 1)
      {
        _previousFrameIndex = 0;
      }

      _previousFrames[_previousFrameIndex] = _fps;
      _averageFPS = _previousFrames.Average();

      _frames = 0;
      _framewatch.Restart();
    }
  }
}