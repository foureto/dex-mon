import ky, { Options } from "ky";
import { notification } from "antd";
import {
  AnyObject,
  IResult,
  isFormData,
  JSONHelper,
  Nullable,
} from "./commons";

type Payload = Nullable<AnyObject | FormData>;

export abstract class ServiceBase {
  protected static PREFIX = "";
  protected static TIMEOUT: Options["timeout"] = false;

  protected static api = ky.create({
    timeout: this.TIMEOUT,
    hooks: {
      beforeRequest: [
        (request, _) => {
          request.headers.set("ngrok-skip-browser-warning", "69420");
        },
      ],
      afterResponse: [
        async (_, __, response) => {
          if (response.status === 401 || response.status === 403) {
            return location.replace("/login");
          }

          const data: Nullable<IResult> = await response.json();
          if (data?.message) {
            data.success
              ? notification.success({
                  message: "Success",
                  description: data.message,
                })
              : notification.error({
                  message: "Error",
                  description: data.message,
                });
          }
        },
      ],
      beforeError: [
        async (error) => {
          const data: Nullable<IResult> = await error.response.json();
          if (data?.message) {
            notification.error({
              message: error.response.statusText,
              description: data.message,
            });
          }
          return error;
        },
      ],
    },
  });

  private static http<T = unknown>(
    method: string,
    url: string,
    payload?: Payload
  ) {
    return this.api<T>(url, { method, ...this.getOptions(payload) }).json();
  }

  private static async stream<T = unknown>(
    method: string,
    url: string,
    payload?: Payload,
    callback?: (e: any) => void
  ): Promise<{ success: boolean }> {
    const res = await this.api<T>(url, {
      method,
      ...this.getOptions(payload),
      timeout: false,
    });

    if (!res.body) {
      return new Promise((resolve) => resolve({ success: false }));
    }

    const decoder = new TextDecoder("utf-8");
    for await (const chunk of res.body as any) {
      callback?.(JSONHelper.tryParse(decoder.decode(chunk)));
    }

    return new Promise((resolve) => resolve({ success: true }));
  }

  protected static get<T = unknown>(url: string, payload?: Payload) {
    if (!payload) return this.http<T>("GET", url);
    const query = Object.getOwnPropertyNames(payload)
      .reduce((res: string[], key: string) => {
        const pld = payload as any;
        if (Array.isArray(pld[key])) {
          if (pld[key].length > 0)
            res.push(pld[key].map((v) => `${key}=${v}`).join("&"));
          return res;
        }
        if (pld[key] === null || pld[key] === undefined) return res;
        res.push(`${key}=${pld[key]}`);
        return res;
      }, [])
      .join("&");

    return this.http<T>("GET", `${url}?${query}`);
  }
  protected static post<T = unknown>(url: string, payload?: Payload) {
    return this.http<T>("POST", url, payload);
  }
  protected static put<T = unknown>(url: string, payload?: Payload) {
    return this.http<T>("PUT", url, payload);
  }
  protected static delete<T = unknown>(url: string, payload?: Payload) {
    return this.http<T>("DELETE", url, payload);
  }

  protected static streamPost<T = unknown>(
    url: string,
    payload?: Payload,
    cb?: (e: any) => void
  ) {
    return this.stream<T>("POST", url, payload, cb);
  }

  private static getPayload = (data?: Payload): Options => {
    if (!data) return {};
    return isFormData(data) ? { body: data } : { json: data };
  };
  private static getPrefix() {
    if (!this.PREFIX) return "/api/";
    return `/api/${this.PREFIX}`;
  }
  private static getOptions(data?: Payload): Options {
    return {
      ...this.getPayload(data),
      prefixUrl: this.getPrefix(),
      timeout: this.TIMEOUT,
    };
  }
}
