import {FC, ReactNode} from "react";
import {APIContext} from "./APIContext.tsx";
import {AuthContextProvider} from "./AuthContext.tsx";

type Props = {
  children: ReactNode
}

export const AppContextProvider: FC<Props> = ({children}) => {
  return (
    <APIContext>
      <AuthContextProvider>
        {children}
      </AuthContextProvider>
    </APIContext>
  )
}
