import {AxiosError} from "axios";

type HttpErrorHandlerProps = {
  err: AxiosError,
  errorKeys: string[],
  setFieldErrors?: Record<string, (msg: string) => void>,
  onCustomError?: () => void,
}

const showErrorToast = ({ title, message }: {title: string, message: string}) => {
  // TODO: add UI toasts
  console.error(title, message);
}

export const httpErrorHandler = ({
                                   err,            // The error object from the HTTP response
                                   errorKeys,      // Array of keys to look for in the error response
                                   setFieldErrors, //Function to set field-specific errors
                                   onCustomError,  // Optional custom error handling function
                                 }: HttpErrorHandlerProps) => {
  // Call the custom error handling function if provided
  onCustomError?.();

  // Define different error cases and their corresponding handlers
  const errorCases: Record<number | 'default', () => void> = {
    400: () => handleBadRequest(err, errorKeys, setFieldErrors), // Handle bad request errors (400)
    401: () => handleUnauthorized(),                      // Handle unauthorized errors (401)
    403: () => showErrorToast({                                  // Handle forbidden errors (403)
      title: 'Unauthorized',
      message: 'Permission denied'
    }),
    404: () => showErrorToast({                                  // Handle not found errors (404)
      title: 'Not Found',
      message: 'Requested resource not found'
    }),
    500: () => showErrorToast({                                  // Handle internal server errors (500)
      title: 'Server Error',
      message: 'Internal Server Error'
    }),
    default: () => showErrorToast({                              // Handle any other unspecified errors
      title: 'Error',
      message: 'Something went wrong'
    }),
  };

  // Determine the error handler based on the error status or use the default handler
  const handleCustomError = errorCases[err?.status as number] || errorCases.default;
  // Execute the determined error handler
  handleCustomError();
};

// Function to handle bad request errors (400)
const handleBadRequest = (
  err: HttpErrorHandlerProps['err'],
  errorKeys: HttpErrorHandlerProps['errorKeys'],
  setFieldErrors: HttpErrorHandlerProps['setFieldErrors']) => {
  console.error(err);
  console.error(errorKeys);
  console.error(setFieldErrors);
  /* TODO:
  // Iterate over the specified error keys
  errorKeys?.forEach(key => {
    // Get the first error message for each key from the error response
    const firstError = err?.data?.[key]?.[0];
    if (firstError) {
      // Set the field-specific error using the provided setFieldErrors function
      setFieldErrors?.[key](firstError);
    }
  });
   */
};

// Function to handle unauthorized errors (401)
const handleUnauthorized = () => {
  // Show an error toast to notify the user about token expiration
  showErrorToast({
    title: 'Error',
    message: 'Token expired. Please login again.',
  });
  // Clear any stored authentication data
  localStorage.removeItem("token");
  // Update the authentication state in the context
  //context?.setIsAuthenticated(false);
};

