import numpy as np
import cv2
from find_obj import filter_matches,explore_match

cap = cv2.VideoCapture(0)
searchFor = cv2.imread('myPic.jpg',0)

while(True):
	# Capture frame-by-frame
	ret, frame = cap.read()
	searchIn = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

	img1 = searchFor
	img2 = searchIn

	# Initiate SIFT detector
	orb = cv2.ORB()

	# find the keypoints and descriptors with SIFT
	kp1, des1 = orb.detectAndCompute(img1,None)
	kp2, des2 = orb.detectAndCompute(img2,None)

	# create BFMatcher object
	bf = cv2.BFMatcher(cv2.NORM_HAMMING)#, crossCheck=True)

	matches = bf.knnMatch(des1, trainDescriptors = des2, k = 2)
	p1, p2, kp_pairs = filter_matches(kp1, kp2, matches)
	explore_match('find_obj', img1,img2,kp_pairs)#cv2 shows image

	# Check For Close
	if cv2.waitKey(1) & 0xFF == ord('q'):
		break

# When everything done, release the capture
cap.release()
cv2.destroyAllWindows()
