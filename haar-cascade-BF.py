import numpy as np
import cv2
from find_obj import filter_matches,explore_match

face_cascade = cv2.CascadeClassifier('haarcascades/haarcascade_frontalface_default.xml')
eye_cascade = cv2.CascadeClassifier('haarcascades/haarcascade_eye.xml')

orb = cv2.ORB() # Init SIFT detector
bf = cv2.BFMatcher(cv2.NORM_HAMMING) # Create BF Matcher

searchFor = cv2.imread('myPic.jpg',0) # Load Search Image
#searchForGray = cv2.cvtColor(searchFor, cv2.COLOR_BGR2GRAY) # Change to GRAY
kpFor, desFor = orb.detectAndCompute(searchFor,None) # SIFT Detect Search Image

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
		croppedGray = cv2.cvtColor(cropped, cv2.COLOR_BGR2GRAY)
		
		# Match Face
		kpIn, desIn = orb.detectAndCompute(croppedGray,None)
		matches = bf.knnMatch(desFor, trainDescriptors = desIn, k = 2)
		pFor, pIn, kp_pairs = filter_matches(kpFor, kpIn, matches)
		explore_match('Match', searchFor,croppedGray,kp_pairs) # CV2 shows image

	# Show Result
	cv2.imshow('Raw Image',img)

	# Check For Close
	if cv2.waitKey(1) & 0xFF == ord('q'):
		break

# When everything done, release the capture
cap.release()
cv2.destroyAllWindows()
