
CREATE TABLE Courses (
  CourseID int NOT NULL,
  Name varchar(100),
  Capacity int,
  
  CONSTRAINT PK_Courses PRIMARY KEY (CourseID)
);

CREATE TABLE Students (
  StudentId int NOT NULL,
  Name varchar(100),

  CONSTRAINT PK_Students PRIMARY KEY  (StudentId)
);

CREATE TABLE Schedule (
  CourseId int NOT NULL,
  CourseCapacity int NOT NULL,
  CONSTRAINT PK_Schedule PRIMARY KEY (CourseId),
  CONSTRAINT FK_Courses_Schedule FOREIGN KEY (CourseId)
  REFERENCES Courses (CourseID)
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE StudentCourses (
  StudentId int NOT NULL,
  CourseId int NOT NULL,
  
  CONSTRAINT PK_StudentCourses PRIMARY KEY (StudentId),
  CONSTRAINT FK_Students FOREIGN KEY (StudentId)
  REFERENCES Students (StudentId)
  ON DELETE CASCADE
  ON UPDATE CASCADE,
  
  CONSTRAINT FK_Courses FOREIGN KEY (CourseId)
  REFERENCES Courses (CourseID)
  ON DELETE CASCADE
  ON UPDATE CASCADE,
  CONSTRAINT AK_CourseId UNIQUE(StudentId, CourseId)
