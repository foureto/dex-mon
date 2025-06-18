import { redirect } from "react-router-dom";
import { IDataResult } from "./commons";

const openNotificationSuccess = (_: string) => {
  return;
};

const openNotificationError = (_: string) => {
  return;
};

export abstract class ServiceBase {
  protected static BASE_URL = "";

  protected static getUrl(url: string): string {
    return `/api/${this.BASE_URL}${url}`;
  }

  protected static async http(
    method: string,
    url: string,
    data?: any,
    otherHeaders?: any
  ): Promise<any> {
    const headers = {
      Accept: "application/json",
      "Content-Type": "application/json",
      "x-app-chk": "bot[6]",
      ...otherHeaders,
    };

    if (headers && headers["Content-Type"] === null) {
      delete headers["Content-Type"];
    }

    const response = await fetch(this.getUrl(url), {
      method,
      body:
        method === "GET"
          ? null
          : headers["Content-Type"]
            ? JSON.stringify(data)
            : data,
      credentials: "include",
      headers,
      redirect: "follow",
    });

    return new Promise(async (resolve, reject) => {
      if (
        response.headers.get("content-type") === "application/pdf" ||
        !!response.headers.get("content-disposition")
      ) {
        const res = await response.blob();
        return resolve(res);
      }
      if (response.status === 401) {
        openNotificationError("You are not authorized");
        redirect("/login");
        return reject("You are not authorized");
      }

      try {
        const result = await response.json();
        if (result.success === false) {
          if (result?.message) openNotificationError(result.message);
          return reject(`error.${result.errorCode}`);
        }

        if (result?.message) openNotificationSuccess(result.message);
        return resolve(result);
      } catch {
        return reject("Bad response");
      }
    });
  }

  protected static async stream<T>(
    method: string,
    url: string,
    data?: any,
    callback?: (e: IDataResult<T>) => void
  ): Promise<any> {
    const headers = {
      Accept: "application/json",
      "Content-Type": "application/json",
    };

    const enc = new TextDecoder("utf-8");
    const response = await fetch(this.getUrl(url), {
      method,
      body: headers["Content-Type"] ? JSON.stringify(data) : data,
      credentials: "include",
      headers,
    });

    let last: IDataResult<T> = {
      success: false,
      message: "",
      statusCode: 0,
    };
    
    for await (const chunk of response.body as any) {
      last = JSON.parse(enc.decode(chunk));
      callback?.(last);
    }

    return new Promise((resolve) => {
      return resolve({ success: true });
    });
  }

  protected static get(url: string, data?: any, options?: any): any {
    if (data) {
      [...Object.getOwnPropertyNames(data)].forEach((e) => {
        if (data[e] === undefined) delete data[e];
      });
    }

    const query = data ? `?${new URLSearchParams(data).toString()}` : "";
    return this.http("GET", url + query, data, options);
  }

  protected static post(url: string, data?: any, options?: any): any {
    return this.http("POST", url, data, options);
  }

  protected static put(url: string, data?: any, options?: any): any {
    return this.http("PUT", url, data, options);
  }

  protected static delete(url: string, data?: any, options?: any): any {
    return this.http("DELETE", url, data, options);
  }

  protected static streamPost<T>(
    url: string,
    data?: any,
    callback?: (e: IDataResult<T>) => void
  ): any {
    return this.stream("POST", url, data, callback);
  }
}
