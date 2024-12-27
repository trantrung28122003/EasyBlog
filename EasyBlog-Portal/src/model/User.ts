
export interface User {
  id: string;
  userName: string;
  email: string;
  fullName: string;
  dayOfBirth: any;
  imageUrl: any;
  roles: Role[];
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
