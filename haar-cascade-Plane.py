import numpy as np
import cv2
from common import getsize, draw_keypoints
from plane_tracker import PlaneTracker

face_cascade = cv2.CascadeClassifier('haarcascades/haarcascade_frontalface_default.xml')
eye_cascade = cv2.CascadeClassifier('haarcascades/haarcascade_eye.xml')

searchFor = cv2.imread('myPic.jpg',0) # Load Search Image
#searchForGray = cv2.cvtColor(searchFor, cv2.COLOR_BGR2GRAY) # Change to GRAY
searchHeight, searchWidth = searchFor.shape

tracker = PlaneTracker()
tracker.add_target(searchFor, (0, 0, searchWidth, searchHeight))

cap = cv2.VideoCapture(0) # Init Camera

while(True):
	# Get Frame
	ret, img = cap.read()
	gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

	faces = face_cascade.detectMultiScale(gray, 1.3, 5)
	for (x,y,w,h) in faces:
		cv2.rectangle(img,(x,y),(x+w,y+h),(255,0,0),2) # Draw Faces On img

		# Crop Face
		cropped = img[y:y+h, x:x+w]
		#croppedGray = cv2.cvtColor(cropped, cv2.COLOR_BGR2GRAY)
		
		
		tracked = tracker.track(cropped)
		print len(tracked)


	# Show Result
	cv2.imshow('Raw Image',img)

	# Check For Close
	if cv2.waitKey(1) & 0xFF == ord('q'):
		break

# When everything done, release the capture
cap.release()
cv2.destroyAllWindows()
