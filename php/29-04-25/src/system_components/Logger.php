<?php

namespace SystemComponents;

class Logger
{
    private static $logFile = __DIR__ . '/../logs/app.log';
    private static $routeFile = __DIR__ . '/../logs/route.log';

    private static $initFlagFile = __DIR__ . '/../logs/logger.initialized';
    private static $initialized = false;

    const LEVEL_ERROR = 'ERROR';
    const LEVEL_DEBUG = 'DEBUG';
    const LEVEL_ROUTE = 'ROUTE';

    public static function init()
    {
        if (self::$initialized) {
            return;
        }

        if (file_exists(self::$initFlagFile)) {
            self::$initialized = true;
            return;
        }

        if (!file_exists(dirname(self::$logFile))) {
            mkdir(dirname(self::$logFile), 0777, true);
        }

        if (!file_exists(self::$logFile)) {
            touch(self::$logFile);
        }

        if (!file_exists(self::$routeFile)) {
            touch(self::$routeFile);
        }

        touch(self::$initFlagFile);
        self::$initialized = true;

        self::logDebug('Logger initialized');
    }
    public static function log($message, $level = self::LEVEL_ERROR)
    {
        $timestamp = date('Y-m-d H:i:s');
        $formattedMessage = "[$timestamp] [$level] $message" . PHP_EOL;

        file_put_contents(self::$logFile, $formattedMessage, FILE_APPEND);
    }

    public static function logError($message)
    {
        self::log($message, self::LEVEL_ERROR);
    }

    public static function logDebug($message)
    {
        self::log($message, self::LEVEL_DEBUG);
    }

    public static function logException(\Exception $exception)
    {
        $message = "Exception caught: " . $exception->getMessage() . " in " . $exception->getFile() . " on line " . $exception->getLine();
        self::logError($message);
    }

    public static function registerErrorHandler()
    {
        set_error_handler(function ($errno, $errstr, $errfile, $errline) {
            $message = "PHP Error [$errno]: $errstr in $errfile on line $errline";
            self::logError($message);
        });
    }

    public static function registerExceptionHandler()
    {
        set_exception_handler(function ($exception) {
            self::logException($exception);
        });
    }

    public static function route($url, $level = self::LEVEL_ROUTE)
    {
        $timestamp = date('Y-m-d H:i:s');
        $formattedMessage = "[$timestamp] [$level] $url" . PHP_EOL;

        file_put_contents(self::$routeFile, $formattedMessage, FILE_APPEND);
    }
}
