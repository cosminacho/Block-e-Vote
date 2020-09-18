import sys
import json
import face_recognition


class ShellResult:
    def __init__(self, success, result, message):
        self.Success = success
        self.Result = result
        self.Message = message


# # Load the jpg files into numpy arrays
# biden_image = face_recognition.load_image_file("biden.jpg")
# obama_image = face_recognition.load_image_file("obama.jpg")
# unknown_image = face_recognition.load_image_file("nacho2.jpeg")
# nacho_image = face_recognition.load_image_file("nacho.jpg")

# # Get the face encodings for each face in each image file
# # Since there could be more than one face in each image, it returns a list of encodings.
# # But since I know each image only has one face, I only care about the first encoding in each image, so I grab index 0.
# try:
#     biden_face_encoding = face_recognition.face_encodings(biden_image)[0]
#     obama_face_encoding = face_recognition.face_encodings(obama_image)[0]
#     unknown_face_encoding = face_recognition.face_encodings(unknown_image)[0]
#     nacho_face_encoding = face_recognition.face_encodings(nacho_image)[0]
# except IndexError:
#     print("I wasn't able to locate any faces in at least one of the images. Check the image files. Aborting...")
#     quit()

# known_faces = [
#     biden_face_encoding,
#     obama_face_encoding,
#     nacho_face_encoding
# ]

# # results is an array of True/False telling if the unknown face matched anyone in the known_faces array
# results = face_recognition.compare_faces(known_faces, unknown_face_encoding)

# print("Is the unknown face a picture of Biden? {}".format(results[0]))
# print("Is the unknown face a picture of Obama? {}".format(results[1]))
# print("Is the unknown face a picture of Nacho? {}".format(results[2]))
# print("Is the unknown face a new person that we've never seen before? {}".format(
#     not True in results))


def register_face(image_path, username):
    registered_face = face_recognition.load_image_file(image_path)
    try:
        registered_face = face_recognition.face_encodings(registered_face)[0]
    except IndexError:
        print(json.dumps(ShellResult(False, False,
                                     "Couldn't identify any face in the image.").__dict__))
        quit()

    with open("C:\\Users\\acosm\\Desktop\\EVoting\\EVoting.Face\\faces.txt", "r") as f:
        faces = f.readlines()
        faces = [face[:-1].split(",") for face in faces]

    try:
        for face in faces:
            if face[1] == username:
                print(json.dumps(ShellResult(True, False,
                                             "Face already registered.").__dict__))
                quit()
            face[0] = face_recognition.load_image_file(face[0])
            face[0] = face_recognition.face_encodings(face[0])[0]
    except IndexError:
        print(json.dumps(ShellResult(False, False,
                                     "Couldn't identify a face in the dataset.").__dict__))
        quit()
    known_faces = [face[0] for face in faces]
    result = face_recognition.compare_faces(known_faces, registered_face)
    for item in result:
        if item == True:
            print(json.dumps(ShellResult(True, False,
                                         "Face already reistered.").__dict__))
            quit()

    with open("C:\\Users\\acosm\\Desktop\\EVoting\\EVoting.Face\\faces.txt", "a") as f:
        f.write(image_path)
        f.write(",")
        f.write(username)
        f.write("\n")
    print(json.dumps(ShellResult(True, True, "Face registered succesfully").__dict__))


def verify_face(image_path, username):
    with open("C:\\Users\\acosm\\Desktop\\EVoting\\EVoting.Face\\faces.txt", "r") as f:
        faces = f.readlines()
        faces = [face[:-1].split(",") for face in faces]

    to_check_face = face_recognition.load_image_file(image_path)
    try:
        to_check_face = face_recognition.face_encodings(to_check_face)[0]
    except IndexError:
        print(json.dumps(ShellResult(False, False, "No face found.").__dict__))
        quit()

    for i in range(len(faces)):
        faces[i][0] = face_recognition.load_image_file(faces[i][0])
    try:
        for i in range(len(faces)):
            faces[i][0] = face_recognition.face_encodings(faces[i][0])[0]
    except IndexError:
        print(ShellResult(False, False, "Couldn't identify any face in the dataset."))
        quit()
    known_faces = [face[0] for face in faces]
    results = face_recognition.compare_faces(known_faces, to_check_face)

    for i in range(len(results)):
        if results[i] == True and faces[i][1] == username:
            print(json.dumps(ShellResult(True, True, "Approved!").__dict__))
            quit()
    print(json.dumps(ShellResult(True, False, "Unauthorized!").__dict__))


def main():
    if (len(sys.argv) != 4):
        print(json.dumps(ShellResult(False, False, "Invalid call.").__dict__))
        quit()
    if (sys.argv[3] == "verify"):
        verify_face(sys.argv[1], sys.argv[2])
    elif (sys.argv[3] == "register"):
        register_face(sys.argv[1], sys.argv[2])


if __name__ == "__main__":
    main()
