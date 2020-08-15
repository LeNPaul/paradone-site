/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Javascript functions related to form validation.
//
// The main idea is this: validation functions can be "registered".
// Any validation functions registered will be executed, in the order
// in which they were registered, when VALvalidateFields() is called.
// If one of these validation functions returns false, no more validation
// functions will be executed, and VALvalidateFields will return false.
// If all the validation functions return true, then VALvalidateFields()
// will return true.
//
// Set a form's onsubmit callback to "return VALvalidateFields();".
//
// See component_to_llp.cs for lots of usage examples.
//

var VALfieldValidationFunctions = new Array();

function VALvalidateFields()
{
   // Iterate through registered validation functions
   for (var FunctionIndex = 0; FunctionIndex < VALfieldValidationFunctions.length; ++FunctionIndex)
   {
      if (VALfieldValidationFunctions[FunctionIndex]() == false)
      {
         return false;
      }
   }
   return true;
}

// Register a URL text field validation function. Returns true if the text of the input field is non-null
// and reasonably formatted as a URL.
//
//  * InputId - the id of the <input> element.
//  * RowId - the id of the row containing this input field.
//  * ErrorMessageContainerId - the element with this id will have its innerHTML set to an error message,
//    should the value of the input be invalid.
//  * ShouldValidateCallback - a callback function which should return true if the field should be validated,
//    or false if it shouldn't be (this is for fields which are sometimes not visible, and therefore should
//    not be validated).  If argument is null, it will be ignored.
//  * OnErrorCallback - a callback function which will be called iff the field validation fails.  An
//    example of what this function could do is display the tab containing the invalid field.  If argument
//    is null, it will be ignored.

function VALregisterTextUrlValidationFunction(InputId, RowId, ErrorMessageContainerId, ShouldValidateCallback, OnErrorCallback)
{
   // Set the input element's onchange callback
   var InputField = document.getElementById(InputId);

   if ( ! InputField) {
      return;
   }

   if (InputField.onchange != null)
   {
      var ExistingOnChangeCallback = InputField.onchange;
      InputField.onchange = function()
      {
         ExistingOnChangeCallback();
         VALclearError(RowId, ErrorMessageContainerId);
      };
   }
   else
   {
      InputField.onchange = function()
      {
         VALclearError(RowId, ErrorMessageContainerId);
      };
   }
   
   VALfieldValidationFunctions[VALfieldValidationFunctions.length] = function()
   {
      var InputField = document.getElementById(InputId);
      var TextString = InputField.value;
      
      if (ShouldValidateCallback != null)
      {
         if (ShouldValidateCallback() == false)
         {
            VALclearError(RowId, ErrorMessageContainerId);
            return true;
         }
      }
      
      if (TextString == null || TextString == "")
      {
         VALdisplayError(InputId, RowId, ErrorMessageContainerId, OnErrorCallback, 'Field cannot be left blank.');
         return false;
      }

      if (!(/^http(s|):\/\//i.test(TextString)))
      {
         VALdisplayError(InputId, RowId, ErrorMessageContainerId, OnErrorCallback, 'URL should start with "http://" or "https://"');
         return false;
      }

      return true;
   };

}

// Register an integer validation function.
//  * InputId - the id of the <input> element.
//  * RowId - the id of the row containing this input field.
//  * ErrorMessageContainerId - the element with this id will have its innerHTML set to an error message,
//    should the value of the input be invalid.
//  * ShouldValidateCallback - a callback function which should return true if the field should be validated,
//    or false if it shouldn't be (this is for fields which are sometimes not visible, and therefore should
//    not be validated).  If argument is null, it will be ignored.
//  * OnErrorCallback - a callback function which will be called iff the field validation fails.  An
//    example of what this function could do is display the tab containing the invalid field.  If argument
//    is null, it will be ignored.
//  * MinValue - the minimum possible value of the field.
//  * MaxValue - the maximum possible value of the field.
//  * AllowEnvVars - Allow for integer environment variables that to be used in the field.
//                   (Must have valid accompanying div/span elements with "InputId_preview_div"/"InputId_preview" id's respectively in the dom)
//
// MinValue and MaxValue are optional parameters.  Their defaults are 1 and 2147483647, respectively.
//
function VALregisterIntegerValidationFunction(InputId, RowId, ErrorMessageContainerId, ShouldValidateCallback, OnErrorCallback, MinValue, MaxValue, AllowEnvVars)
{
   MinValue = (MinValue == undefined) ? 1          : MinValue; 
   MaxValue = (MaxValue == undefined) ? 2147483647 : MaxValue;
   
   // Set the input element's onchange callback
   var InputField = document.getElementById(InputId);

   if ( ! InputField) {
      return;
   }

   if (InputField.onchange != null)
   {
      var ExistingOnChangeCallback = InputField.onchange;
      InputField.onchange = function()
      {
         ExistingOnChangeCallback();
         VALclearError(RowId, ErrorMessageContainerId);
      };
   }
   else
   {
      InputField.onchange = function()
      {
         VALclearError(RowId, ErrorMessageContainerId);
      };
   }
   
   VALfieldValidationFunctions[VALfieldValidationFunctions.length] = function()
   {
      var InputField = document.getElementById(InputId);
      var IntString = InputField.value;

      var InputPreviewField = document.getElementById(InputId + "_preview");
      
      var InputPreviewFieldValue = "";
      if (InputPreviewField) {
         InputPreviewFieldValue = InputPreviewField.innerHTML;
      } 
      
      if (ShouldValidateCallback != null)
      {
         if (ShouldValidateCallback() == false)
         {
            VALclearError(RowId, ErrorMessageContainerId);
            return true;
         }
      }
      
      if (IntString == null || IntString == "")
      {
         VALdisplayError(InputId, RowId, ErrorMessageContainerId, OnErrorCallback, 'Field cannot be left blank.');
         return false;
      }
      else if (AllowEnvVars && IntString.search(/\${.*}/g) == 0) {
         // Environment variable.
         var EnvVarValue = InputPreviewFieldValue.replace("Preview: ", "").trim();
         if (EnvVarValue == "" || !VALisInteger(EnvVarValue)) {
            VALdisplayError(InputId, RowId, ErrorMessageContainerId, OnErrorCallback, 'Environment variable must be set to a valid positive integer.');
            return false;
         }
      }
      else if (!VALisInteger(IntString))
      {
        VALdisplayError(InputId, RowId, ErrorMessageContainerId, OnErrorCallback, 'Value must be a valid positive integer.');
        return false;
      }
      else
      {
         var IntValue = parseInt(IntString);
         if (IntValue < MinValue)
         {
            VALdisplayError(InputId, RowId, ErrorMessageContainerId, OnErrorCallback, 'Value cannot be less than ' + MinValue + '.');
            return false;
         }
         else if (IntValue > MaxValue)
         {
            VALdisplayError(InputId, RowId, ErrorMessageContainerId, OnErrorCallback, 'Value cannot be greater than ' + MaxValue + '.');
            return false;
         }
      }
      return true;
   };
}


function VALdisplayError(InputId, RowId, ErrorMessageContainerId, OnErrorCallback, ErrorMessage)
{
   var InputElement = document.getElementById(InputId);
   var ErrorMessageContainer = document.getElementById(ErrorMessageContainerId);
   var Row = document.getElementById(RowId);

   ErrorMessageContainer.innerHTML = '&nbsp;<img src="/images/icon_error.gif" /> ' + ErrorMessage;
   if (OnErrorCallback != null)
   {
      OnErrorCallback();
   }
   
   // We're going to highlight the row ourselves, so we don't want the original
   // onfocus event to take effect.
   var OriginalOnFocus = InputElement.onfocus;
   InputElement.onfocus = '';
   InputElement.focus();
   InputElement.onfocus = OriginalOnFocus;
   
   CLSelementAddClass(Row, 'error_row');
}

function VALclearError(RowId, ErrorMessageContainerId)
{
   var ErrorMessageContainer = document.getElementById(ErrorMessageContainerId);
   var Row = document.getElementById(RowId);
   
   ErrorMessageContainer.innerHTML = '';
   CLSelementRemoveClass(Row, 'error_row');
}

function VALisInteger(String)
{
    for (var CharIndex = 0; CharIndex < String.length; ++CharIndex)
    {
        var Char = String.charAt(CharIndex);

        if (!VALisDigit(Char))
        {
           return false;
        }
    }

    return true;
}

function VALisDigit(Char)
{
    return ((Char >= "0") && (Char <= "9"));
}
