KinectSkeletalApp
=================

This simple application shows how to properly use [Kinect for Windows SDK](http://www.microsoft.com/en-us/kinectforwindows/ "Kinect for Windows SDK") in the WPF with Skeletal tracking mode.

Skeletal tracking is one of the most interesting feature of Kinect device. It allow detect person in front of the device. Programmers have an opportunity to write applications which behaviour depends of person movements.

**How does it work?**

First, we have to define a container for skeletons. Kinect device can detect max 6 skeletons at the same time, but only two of them will be marked as active:

`const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];`

We have to enable the `SkeletonStream` as follow:

`_sensor.SkeletonStream.Enable(parameters);`

In `_sensor_AllFramesReady` event method, we have to catch the first skeleton and assign it:

`Skeleton first = GetFirstSkeleton(e);`

In `GetFirstSkeleton()` we use `SkeletonFrame`

`using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())`

Copy important data to assigned variable:

`skeletonFrameData.CopySkeletonDataTo(allSkeletons);`

Next we use `LINQ` to catch the skeleton as follow:

`Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();
`

The last step is to call for example `GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)` method to assign some WPF components to each part of the body:

`DepthImagePoint headDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.Head].Position);`

`Canvas.SetLeft(element, point.X - element.Width / 2);`

**More examples**

Feel free to visit my homepage [Tomasz Kowalczyk](http://tomek.kownet.info/ "Tomasz Kowalczyk") to see more complex examples.