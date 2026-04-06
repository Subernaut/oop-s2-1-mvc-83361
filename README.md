o How to run the app locally

1. Clone the repository
git clone https://github.com/subernaut/oop-s2-1-mvc-83361.git
cd oop-s2-1-mvc-83361
2. Apply database \& run
dotnet ef database update
dotnet run
3. Open in browser
https://localhost:xxxx

Alternative way (If Visual Studio is installed) :

1. Clone the repository
git clone https://github.com/subernaut/oop-s2-1-mvc-83361.git
cd oop-s2-1-mvc-83361
2. Open the file oop-s2-1-mvc-83361.sln
3. Run "dotnet ef database update"
4. Click the "Start" button, or use Ctrl+F5

o How to run tests

Enter the command:
dotnet test

Alternative way (If Visual Studio is installed) :

1. When inside visual studio, go to the "Test" tab in the upper navigation bar,
2. Click "Run All Tests"

o Seeded Demo Accounts (Admin / Faculty / Student)



| Role | Email | Password |

| ----------- | ----------- | ----------- |

| Admin | admin@college.com | Admin123! |

| Faculty | faculty1@college.com | Faculty123! |

| Faculty | faculty2@college.com | Faculty123! |

| Faculty | faculty3@college.com | Faculty123! |

| Student | student1@college.com | Student123! |

| Student | student2@college.com | Student123! |

| Student | student3@college.com | Student123! |

| Student | student4@college.com | Student123! |

| Student | student5@college.com | Student123! |

| Student | student6@college.com | Student123! |



o Design decisions / assumptions

For the accounts creation, I favored utility over realism, as having the same kind of password for all users as well as numbered names (1,2,3...) makes it easier to log into each accounts

I decided that, for the accounts level of access :

Students could only see their own information (such as assignments, exams, results (except unreleased ones), and their own profile), as Students should not be able to change anything (though they should still be able to view information, as to verify administrators or faculty members have not entered something false)

Faculty have a limited access to information, can view and create assignments for their courses (not edit, if they entered an assignment wrong they should seek an administrator), they can view edit and create new assignments' results for their students (the results can be edited since there are more of them, and thus faculty members may be more likely to make mistakes on this subject). However, faculty members are only able to view exams and their results, as these are too important to allow regular faculty members to create/edit/delete them. Another reason is that they happen rarely. They can create/edit/view their students' attendance, as attendance happens regularly. They can view their courses, especially the students that take part in their courses. They can view course enrolments, to view which students left their courses (so that they do not have to take action when they see these students are not present). Finally, they can view their own profile (their own Name, Email, Phone, as registered in the database), and their students' profiles (though they have a limited view, they can only see their students' email, Name, and Student Number, for privacy reasons)

Admins are able to access everything, as they manage the database (can create/delete/edit and view everything). They have exclusive views into the branches, the users' page, and can view fully the Faculty page (which holds the faculty profiles)

I assumed the "grades" option had to be calculated automatically based on Score/Exam.MaxScore

