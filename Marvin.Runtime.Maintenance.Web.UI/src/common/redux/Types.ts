interface Action<T> {
    type: string;
    payload?: T;
  }
  
  export type ActionType<T> = Action<T>;