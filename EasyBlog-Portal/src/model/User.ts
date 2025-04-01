
export interface User {
  id: string;
  userName: string;
  email: string;
  fullName: string;
  dayOfBirth: any;
  imageUrl: any;
  role: string;
  dateCreate: string;
  dateChange: string;
  changedBy: string;
  deleted: boolean;
}

export interface Role {
  id: string;
  name: string;
}

export interface UserNote {
  id: string;
  noteContent: string,
  timeStamp: number, 
  trainingPartName: string,
  trainingPartId: string,
  courseId: string,
  courseEventName: string,
};

export interface UserProfile {
  id: string;
  fullName: string,
  email: number, 
  phoneNumber: string,
  address: string,
  avatar: string,
};


export interface Notification {
  id: string;
  Message: string,
  IsRead: boolean, 
  Type: string,
  DateCreate: string,
};


