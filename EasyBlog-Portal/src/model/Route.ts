export interface ApplicationRoute {
  path: string;
  component: any;
  isProtected: boolean;
  isAdmin: boolean;
}
