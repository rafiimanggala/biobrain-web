import { Injectable } from '@angular/core';

@Injectable()
export class StringsService {
  aAssigned = "A - Assigned";
  accessCode = "Access code";
  active = 'Active';
  activeCodes = 'Active codes'
  accessCodes = 'Access Codes';
  accessDenied = 'access denied';
  add = 'Add';
  addAccessCodes = 'Create Access Codes';
  addBack = "Add back";
  addExisting = 'Add existing';
  addImage = 'Add image';
  additionalSubjects = 'Additional subjects'
  addNewQuestions = 'Add new questions';
  addNewQuestionsConfirmation = 'Are you sure? This action will add all questions that are not added from IB course for all linked levels';
  addNewQuestionsTooltip = 'Add all questions that are not added for all linked levels';
  address = 'Address';
  addSubject = 'Add a subject';
  addSubscription = 'Add new subscription';
  addTeacher = 'Add teacher';
  amount = 'Amount';
  amountUsed = 'Amount used';
  annually = 'annually';
  apply = 'Apply';
  applied = 'Applied';
  archive = 'Archive';
  assignAll = 'Assign all';
  assigned = "Assigned";
  assignments = "Assignments";
  assignedShort = 'A';
  assignMaterial = 'Assign material';
  assignQuiz = 'Assign quiz';
  atomic = "Atomic";
  atomicMass = 'Atomic Mass';
  attachContent = 'Link Content';
  attachMaterial = 'Link Material';
  attachQuestion = 'Link Question';
  availableInDemo = 'Available in demo';
  autoMapOptions = "Automap Options";

  back = "Back";
  batch = 'Batch';
  biology = 'Biology';
  biobrainLink = 'go.biobrain.tech/login';
  baseCourse = 'Base course';

  cancelSubscription = 'Cancel subscription';
  cancelSubscriptionConfirmation = 'Are you sure you wan\'t to cancel this subscription?';
  cancelSubjectsText = 'Please select the subject/s to cancel and confirm.';
  subjectCanceled = '(canceled)';
  cardNumber = 'Card Number';
  chemistry = 'Chemistry';
  city = 'City';
  classAdmin = 'Class admin';
  classCode = 'Class code';
  classCodeNotAvailableError = 'Signing up with class code not available for this school. This school use access codes.';
  clearContent = "Clear local content";
  comingSoon = "Coming soon";
  confirm = 'confirm';
  content = 'Content';
  contentExpiryDate = 'Content expiry date';
  contentLoader = 'Content Loader';
  contentMapper = 'Content Mapper';
  contentReport = 'Content Report';
  chooseSubscription = 'Choose Subscription';
  chooseSubjects = 'Choose Subjects';
  chooseYourSubjects = 'Choose your subjects';
  contactUs = 'Contact Us';
  complete = "Complete";
  couldntFind = "Couldn’t find what you need?";
  country = 'Country';
  countryNotSelected = 'Country must be selected from list';
  course = 'Course';
  courses = 'Courses';
  courseNotAvailable = 'This course not available';
  correctAnswer = 'Correct answer';
  correctAnswerIs = 'Correct answer is';
  create = 'Create';
  createQuiz = 'Create Quiz';
  createdAt = 'Created at';
  curriculum = 'Curriculum';
  customQuiz = 'Custom Quiz';
  customQuizDescription = 'Create a personalised quiz by selecting topics and number of questions.';
  createCustomQuiz = 'Create Quiz';
  cvc = 'CVC';

  delete = 'Delete';
  deleteAccount = 'Delete account';
  deleteAccountConfirmation = 'Are you sure you want to delete this account and all subscriptions?';
  dontAutoMap = "Don't auptomap";
  downloadingContent = "Downloading Content";
  downloadActiveCodes = "Download active codes";
  downloadUsedCodes = "Download Used Codes";
  downloadVouchers = "Download Vouchers";
  downloadResults = "Download class results";
  enableHints = 'Enable Hints';
  enableSound = 'Enable Sound Effects';
  disableHintsForClass = 'Disable hints for this class';
  disableSoundForClass = 'Disable sound effects for this class';
  classSettings = 'Class settings';
  soundOn = 'Sound On';
  soundOff = 'Sound Off';
  anonymizeResults = 'Anonymize Results';
  includeLearningMaterial = 'Include in assignment';
  dropDown = "Dropdown";
  dropFilesMessage = "Drop files or click to select.";

  edit = 'Edit';
  editStudent = 'edit student';
  editYourBillingDetails = 'Edit your billing details';
  emailClass = 'Email class';
  endDate = 'End Date';
  enjoyYourPaymentPlan = 'Enjoy your payment plan';
  exclude = "Exclude";
  expiryDate = 'Expiry Date';
  expiryDateChangeConfirmation(date: string) {return `Are you sure you want to change expiry date to: ${date}?`;}
  expiryMonth = 'Expiry Month';
  expiryYear = 'Expiry Year';
  expires = 'expires';

  forensics = 'Forensics';
  forgotPassword = 'Forgot password?';
  freeText = 'Short Answer';
  freeTrial = 'Free trial';
  freeTrialPeriodConcluded = 'Your free trial period has concluded.\nTo purchase a subject or subjects, please click the button below or enter an access code';
  freeTrialPeriodConcludedTeacher = 'Your free trial period has concluded.\nTo purchase a class, subject or school subscription, please contact <a href="mailto:support@biobrain.com.au?subject=BioBrain%20-%20subscription">support@biobrain.com.au<a>';
  from = "From";

  generic = 'generic';
  get = 'Get';
  glossary = 'Glossary';
  goToPreview = "Go to preview";
  grade11_12 = '11th/12th Grade';
  grade9_10 = '9th/10th Grade';
  groups = "Groups";

  haveAccessCode = 'I have an access code';
  haveClassCode = 'I have a class code';
  haveVoucher = 'I have a voucher';
  hotTip = 'hot tip...';

  include = 'Include';
  invalidEmail='Invalid email';
  inviteStudentByEmail = 'Invite student by email';
  isAvailableForStudent = "Is available for student";

  // j

  // k

  lastUpload = 'Last upload';
  lastRelease = 'Last release';
  learningMaterials = 'Learning materials';
  license = 'License';
  live = "Live";
  liveCustomer = 'Live customer';
  login = 'Log in';
  loginWithSso = 'Log in with SSO';
  logout = 'Log out';
  ssoSchoolNamePlaceholder = 'Enter your school name';
  ssoSearching = 'Searching...';
  ssoNoSchoolFound = 'No SSO-enabled school found.';
  ssoError = 'SSO login failed. Please try again.';
  ssoLogin = 'Continue with SSO';

  mapFrom = "Map From";
  marine = "Marine";
  materials = 'materials';
  missParametersError = 'Miss required page parameters';
  mm = 'mm';
  myAccount = 'My Account';
  mySavedItems = 'My Saved Items';
  mySubjects = 'My Subjects';
  month = 'month';
  monthly = 'monthly';
  moveDown = 'Down';
  moveUp = 'Up';
  moveStudent = 'move student';
  multipleChoice = 'Multiple Choice';

  name = 'Name';
  nameOnCard = 'Name on Card';
  naNotAssigned = "NA - Not Assigned";
  next = 'Next';
  nextLevel = 'Next Level';
  nextPayment = 'Next payment';
  nextTopic = 'Next Topic';
  noContentAvailable = 'No content available for this course.';
  noCoursesHeader = 'Purchase courses';
  noCoursesMessage = 'You don\'t have active subscriptions or school classes. To purchase a subject or subjects, please click the button below';
  node = 'Node';
  numberOfQuestions = 'Number of Questions';
  note = 'Note';
  notAssigned = 'Not Assigned';
  notAssignedShort = 'NA';
  notFound = 'Not found';
  noQuestionsToAdd = 'No questions to add';
  noValidSubscription = 'No valid subscription';
  number = "Number";
  numberOfCodes = "Number of codes";
  numberOfStudentsAssigned = '# of students assigned';

  operationCanceled = 'Operation cancelled';
  order = 'order';
  orderList = 'Order List';
  _optional = ' (optional)';

  partiallyComplete = "Partially complete";
  password = 'Password';
  passwordPlaceholder = '************';
  passwordHint = 'Passwords should contain a minimum of 6 characters';
  pasteFromGeneric = 'Paste topic from generic course';
  paymentFailed = 'Payment failed';
  period = 'Period';
  periods = 'Periods';
  periodicTable = 'Periodic Table';
  permanentDeleteUser = "Delete user permanently";
  permanentDeleteUserConfirmation(email: string): string { return `All user related data (quiz results, subscriptions, activated access cedes e.t.c.) will be deleted too. This operation cannot be undone. Delete user ${email} permanently? `};
  permissionDeniedError = 'Permission denied';
  physics = 'Physics';
  promoCode = "Promo code";
  psychology = 'Psychology';
  publish = "Publish";
  publishConfirmation = 'Attention! This action will update this course for all users with access. This action is best performed when there are no active users.';
  publishResult = "Course successfully  published";
  purchase = "Purchase";
  purchaseReport = "Purchase Report";
  purchaseReportMessage = "Information for dates before 02/24/2022 may be incorrect, because the system did not save all the necessary data (if the field contains '-', then the data for this payment was not saved).";

  questions = 'questions';
  questionFeedback = "Thank you for your question, we will send you a response shortly.";
  questionType = 'Question type';
  quiz = 'Quiz';
  quizAssignment = "Quiz Assignment";
  quizName = 'Quiz Name';
  quizNamePlaceholder = 'e.g. My Revision Quiz';
  quizzes = 'Quizzes';

  redeemExpiryDate = 'Redeem Expiry Date';
  registeredAt = 'Registered at'
  relevantQuestionsText = 'You are only seeing questions relevant to your course';
  remove = 'remove';
  removeTeacherForClass = 'Remove teacher from class';
  renameClass = 'Rename class';
  renewalCanceled = 'Renewal cancelled';
  removeFromClass = 'Remove From Class';
  replaceMode = 'Replace Mode';
  results = 'Results';
  response = 'Response';
  requiredFieldError = 'Required field';

  savedItems = 'Saved items';
  schoolManagement = 'School management';
  science = "Science";
  signin = 'Sign in';
  search = 'Search';
  select = 'Select';
  selectAll = 'Select All';
  selectTopics = 'Select Topics';
  sendLogs = 'Send logs';
  subject = 'subject';
  subjects = 'subjects';
  submitQuestion = "Submit my question";
  subscribe = 'Subscribe';
  subscribedAt = 'Subscribed at';
  subscribeNow = 'Subscribe now';
  subscriptions = 'Subscriptions';
  symbol = "Symbol";
  start = 'Start';
  startDate = "Start date"
  started = "Started";
  state = 'State';
  status = 'Status';
  studentsLicenseLimitExceeded = 'Student license limit exceeded';
  success = 'Success';
  startUsingBiobrain = 'Start using BioBrain';
  subsectionQuiz = 'Subsection Quiz';
  swipe = "Swipe";

  takeQuiz = 'Take Quiz';
  takeTopicQuiz = 'Topic Quiz';
  takeSubsectionQuiz = 'Subsection Quiz';
  tasks = 'Tasks';
  template = 'template';
  templates = 'Templates';
  quizTemplates = 'Quiz Templates';
  aiPracticeSet = 'AI Practice Set';
  aiInsights = 'AI Insights';
  text = 'text';
  thanksForSignUpMessage = 'Thanks for signing up to BioBrain';
  to = 'To';
  toEnrolNewStudent = 'To enrol a new student';
  totalBillingToday = 'Total billing today';
  type = "Type";
  typeYourQuestion = "Type your question here and we will email the answer to you.";
  totalCodes = "Total codes";
  trueFalse = "True/False";

  // u
  unassign = "unassign";
  upload = "Upload";
  usageReport = "Usage Report";
  usedCodes = "Used codes";
  useVoucher = "Use voucher";
  useAccessCodes = "Use access codes";
  useAccessCode = "Use Access Code";
  userEmail = "User email";
  userGuides = "User guides";
  unselectAll = 'Unselect All';
  upgradeMessage = "Upgrade your subscription to access all content.";

  // v
  viewQuiz = 'View Quiz';
  viewQuizQuestions = 'View Quiz Questions';
  videoUrlError = 'Video url is not correct';
  video = 'Video';
  voucher = "Voucher";
  vouchers = 'Vouchers';
  vouchersReport = "Vouchers Report";

  // w
  whatAreYouGoingToStudy = 'What are you going to teach today?';
  workAssigned = 'Work Assigned';

  // x

  // y
  yearly='yearly';
  year11_12 = 'Year 11/12';
  year9_10 = 'Year 9/10';
  years9_10 = 'Years 9/10';
  yourEmail = 'Your email';
  yourEmailPlaceholder = 'example@example.com';
  yourPaymentDetails = 'Your payment details';
  yourTotal = 'Your total is: ';
  yy = 'yy';

  // z
  zeroPercent = "0%";

  correct = 'Correct';
  incorrect = 'Incorrect';
  userDataError = 'Invalid user role';
  welcomeBack = 'Welcome Back';
  schools = 'Schools';
  standaloneTeachers = 'standalone Teachers';
  standaloneStudents = 'standalone Students';
  teachers = 'teachers';
  students = 'students';
  teachersLicensesNumber = 'teachers licenses number';
  studentsLicensesNumber = 'students licenses number';
  teachersCount = 'teachers count';
  studentsCount = 'students count';
  editAdmins = 'edit admins';
  editLicenses = 'edit licenses';
  school = 'school';
  cancel = 'Cancel';
  save = 'save';
  yes = 'yes';
  no = 'no';
  wouldYouLikeToDelete = 'Would you like to delete';
  email = 'email';
  userType = 'User type';
  firstName = 'First name';
  material = 'Material';
  question = 'Question';
  lastName = 'Last name';
  teacher = 'teacher';
  class = 'class';
  year = 'year';
  student = 'student';
  classes = 'classes';
  selectTeacher = 'select teacher';
  admins = 'admins';
  mustSelectValue = 'Must Select Value';
  view = 'view';
  nameMustBeUnique = 'name must be unique';
  nameMustBeUniqueForTheSelectedYear = 'name must be unique for the selected year';
  close = 'close';
  unableToPerformOperation = 'unable to perform operation';
  myClasses = 'my classes';
  classResults = 'class results';
  averageQuizScore = 'average quiz score';
  averageQuizScoreHeader = 'Average\nquiz score';
  quizzesCompletedHeader = 'Quizzes\ncompleted';
  studentProgress = 'student progress';
  notApplicableShort = 'NA';
  totalStudents = 'Total students';
  quizScore = 'quiz score';
  quizResults = 'Quiz Results';
  quizResultsSummary = 'Quiz Results Summary';
  emailStudent = 'email student';
  dateCompleted = 'date completed';
  reassignQuiz = 'reassign quiz';
  reassignLearningMaterial = 'reassign learning material';
  successRate = 'success rate';
  reassignLearningMaterialsAndQuizzes = 'reassign learning materials and quizzes';
  totalQuizzes = 'total quizzes';
  total = 'total';
  header = 'header';
  studentResultEmailStudentExplanation = 'Select quizzes and learning material and then email student to advise they have been reassigned';
  hi = 'hi';
  hint = 'Hint';
  ok = 'ok';
  whatAreYouGoingToStudyToday = 'What are you going to study today?';
  wellDone = 'Well done!';
  tryAgain = 'Try again';
  dueDate = 'Due date';
  assignLearningMaterial = 'Assign Materials to Students';
  assignQuizToStudents = 'Assign Quiz to Students';
  assignLearningMaterialsAndQuizzes = 'Assign Materials and Quizzes';
  error = 'error';
  questionPrefix = 'Q';
  score = 'Score';
  date = 'Date';
  bzz = 'buzz...';
  retakeQuiz = 'Retake Quiz';
  study = 'Study';
  editProfile = 'My details';
  unit = 'Unit';
  aos = 'AOS';
  level = 'Level';

  completeStreatMessage = 'Complete a 7 day streak\nto start building your hive!';

  contentNotLoadedMessage = 'Content not loaded yet';
  subjectNotSelectedMessage = 'Subject not selected';
  quizNotFoundMessage = 'Quiz not found';
  incorrectAnswer = 'Incorrect answer. Please content with administrator.';
  clickTheQuestion = 'Click the question to review';

  getStartedWithBiobrain = 'Get started with BioBrain';
  createPassword = 'Create a Password';
  passwordRequirements = 'Password should be at least 8 '; // TODO:
  submit = 'Submit';
  haveAnAccountAlready = 'Have an account already?';
  youHaveSuccessfullyRegistered = 'You have successfully registered';
  wrongClassCode = 'Incorrect Class Code';
  operationCanNotBeCompleted = 'Operation can\'t be completed';
  setYourPassword = 'Set your password';
  resetPassword = 'Reset password';
  resetYourPassword = 'Reset your password';
  unableToChangePassword = 'Unable to change password';
  unableToResetPassword = 'Unable to reset password';
  unableToChangeEmail = 'Unable to change email';
  unableToGetUsageReport = 'Unable to get usage report';
  passwordHasBeenChanged = 'Password has been changed';
  emailHasBeenChanged = 'Email has been changed';
  incorrectPassword = 'Incorrect password';
  resetEmailExplanation = 'Enter your registered email address and we will send you instructions to reset your password';
  resetEmailSuccessMessage = 'Instructions to reset the password have been sent to email';
  selfResetEmailSuccessMessage = 'Instructions to reset the password have been sent to your email';
  saveYearlyMessage = 'Save 42% when you pay yearly!!';
  signUp = 'sign up';
  joinToClass = 'Join a class';
  teach = 'teach';
  changePassword = 'Change password';
  oldPassword = 'old password';
  newPassword = 'new password';
  changeEmail = 'Change Email';
  newEmail = 'new email';
  dateAssigned = 'date assigned';
  assignedWork = 'assigned work';
  feedback = 'Feedback';
  biobrainFeedback = 'BioBrain feedback';

  html = {
    termsOfServiceString: "<span>I agree to BioBrain's <a href=\"https://biobrain.com.au/terms-of-service/\">Terms of Service</a> and <a href=\"https://biobrain.com.au/privacy_policy/\">Privacy Policy</a></span>",

  };

  messages = {
    pleaseSelectStudents: 'Please select student(s)',
    pleaseSelectQuizzesOrLearningMaterials: 'Please select Quizzes or Learning materials',
    areYouSureWantToResetPassword: 'Are you sure want to reset password?',
    emailWasSent: 'Email was sucessfully sent',
    unassignConfirmation: 'Are you sure you want to unassign this work?',
    quizAssignmentConfirmation: 'This quiz has recently been assigned to all students in the class. Do you want to assign it again?',
    materialAssignmentConfirmation: 'This learning material has recently been assigned to all students in the class. Do you want to assign it again?',
    materialAndQuizAssignmentConfirmation: (quizzes: string[], materials: string[]) =>
    `${quizzes.length > 0
      ? quizzes.join(', ') + ' quiz(zes)' + (materials.length > 0 ? ' and ' : '')
      : ''}
     ${materials.length > 0
      ? materials.join(', ') + ' material(s)'
      : ''}
      have already been assigned. Do you want to assign them again?`,
  };

  errors = {
    noCanvasContext: 'Canvas context is not exist',
    userIsNotAssignedToSchool: 'User is not assigned to school.',
    classIsNotSelected: 'Class is not selected.',
    questionWasNotFound: 'Question was not found.',
    quizWasNotFound: 'Quiz was not found.',
    quizIsNotAssignedToThisMaterial: 'Quiz is not assigned to this material.',
    nodeHasNoParent: 'Node has not parent.',
    nodeHasNoChildren: 'Node has no children.',
    courseMustBeSelected: 'Course must be selected.',
    termNotFound: 'Term not found',
    quizHasNoQuestion: 'Quiz has no question.',
    noCurrentUser: 'Current user is undefined.',
    contentTreeNodeWasNotFound: 'Content tree node was not found',
    userIsNotStudent: 'User is not a Student.',
    userIsNotTeacher: 'User is not a Teacher.',
    userIsNotSystemAdministrator: 'User is not a System Administrator.',
    nodeWasNotFound: 'Node was not found.',
    noTeachersToSelect: 'All teachers available have been added.',
    studentAllReadyAdded: 'Student already enrolled in this class.',
    errorRetrivingDataFromServer: "Error getting data from server",
    errorSavingDataOnServer: "Error while saving data on server",

    maxBundleSize: (subjNumber: number): string => `Maximum subjects to buy is ${subjNumber}`,
    routeParameterWasNotFound: (name: string): string => `Route parameter "${name}" was not found`,
  };

  validationErrors = {
    invalidDueDate: 'Invalid due date.',
    invalidEmail: 'Invalid email'
  };

  youJustCompletedStreak = (days: number): string => `you just completed a\n${days} day streak!`;
  studentLicenseLimitExceeded = (limit: number): string => `student license limit (${limit}) exceeded`;
  teachersLicenseLimitExceeded = (limit: number): string => `teachers license limit (${limit}) exceeded`;
  mustBeGreaterThanOrEqualTo = (value: number): string => `must be greater than or equal to ${value}`;
  mustBeLessThan = (value: number): string => `must be less than ${value}`;
  removeFromClassMessage = (name: string):string => `Remove ${name} from class?`;
  removeStudentFromClassSuccessMessage = (name: string):string => `${name} have been excluded from class`;
  renameClassSuccessMessage = (name: string):string => `Class was renamed to ${name}`;
  inviteStudentSuccessMessage = (email: string):string => `${email} was sucessfully invited to BioBrain`;
  addTeacherSuccessMessage = (name: string):string => `Teacher ${name} was sucessfully added`;
  deleteTeacherSuccessMessage = (name: string):string => `Teacher ${name} was sucessfully removed from class`;
  youWereAddedToClass = (name: string) => `You were added to the "${name}" class`;
  subscriptionMessage = (price: string, date: string, period: string, periodAdj: string, showTaxesWarning: boolean) => `You will be charged ${price} per ${period} starting ${date}.\nYour subscription will automatically renew ${periodAdj} on a recurring basis. You can cancel anytime on your My Account page.${showTaxesWarning ? '\nAll prices are GST inclusive unless otherwise stated.' : ''}`;
  myAccountSubscriptionMessage = (price: string, date: string, period: string, periodAdj: string, showTaxesWarning: boolean) => `You’ll be charged ${price} per ${period} starting ${date}. Your subscription will automatically renew ${periodAdj} on a recurring basis.${showTaxesWarning ? '\nAll prices are GST inclusive unless otherwise stated.' : ''}`;
  freeTrialSubscriptionMessage = (date: string) => `Your free trial starts today and finishes on ${date}.`;
  cancelSubscriptionMessage = (name: string): string => `<div>Hello ${name},</div><div>Thank you for being part of the BioBrain community!</div><div>As per your request, your subscription has successfully been cancelled.</div><div>The BioBrain team</div>`;
  confirmTeacherDelete = (name: string) => `Are you sure you want to remove ${name} from the class?`;
}
