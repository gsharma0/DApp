import { Photo } from "./Photo";

export interface member {
    id: number;
    userName: string;
    introduction:string
    photoUrl: string;
    age: number;
    knownAs: Date;
    created: Date;
    lastActive: Date;
    gender: string;
    lookingFor: string;
    interests: string;
    city: string;
    country: string;
    photos: Photo[];
}
