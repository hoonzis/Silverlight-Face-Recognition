Silverlight & Face Recognition
==============================
This projects demonstrates how to create a face recognition application using Silverlight, EmguCV.
Since the version 4 Silverlight allows usage of WebCam in web applications. This applications uses the Silverlight Web cam features to acquire the images.
EmguCV is a managed wrapper for OpenCV library. OpenCV is image processing library which contains several algorithms for image treatment.

This applications demostrates how to perform Face Recognition using the Eigenfaces algorithm (PCA - Principal Component Analysis) which is part of the OpenCV library.
The face detection is done using Haarcascade algorithm. 

The web application exposes a web service which accepts the image either for recognition or storage of the file in the database.
When the service is invoked for the recognition, a "distance" is computed between all the images in the database and the given image, and the closest match is returned.

More information is available at these blogs:
- http://blog.octo.com/en/basics-face-recognition/
- http://blog.octo.com/en/face-recognition-in-web-application/