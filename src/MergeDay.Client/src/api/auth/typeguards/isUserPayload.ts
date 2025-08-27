import {JWTPayload} from "jose";
import {DecodedTokenType} from "../types/decoded-token.ts";

export const isUserPayload = (payload: JWTPayload): payload is DecodedTokenType => {
  return (
    typeof payload.sub === "string" &&
    typeof payload.email === "string" &&
    typeof payload.name === "string" &&
    typeof payload.family_name === "string" &&
    typeof payload.exp === "number" &&
    typeof payload.iss === "string" &&
    typeof payload.aud === "string"
  );
}
